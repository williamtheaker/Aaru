// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Recordable.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : CompactDisc dumping.
//
// --[ Description ] ----------------------------------------------------------
//
//     Handles run-out sectors at end of CD-R(W) discs.
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2022 Natalia Portillo
// ****************************************************************************/

namespace Aaru.Core.Devices.Dumping;

using System.Collections.Generic;
using System.Linq;
using Aaru.CommonTypes.Enums;
using Aaru.CommonTypes.Extents;
using Aaru.CommonTypes.Interfaces;
using Aaru.CommonTypes.Structs;
using Aaru.Core.Logging;
using Aaru.Decoders.CD;
using Aaru.Devices;

partial class Dump
{
    void HandleCdrRunOutSectors(ulong blocks, MmcSubchannel desiredSubchannel, ExtentsULong extents,
                                HashSet<int> subchannelExtents, SubchannelLog subLog, bool supportsLongSectors,
                                Dictionary<byte, byte> trackFlags, Track[] tracks)
    {
        List<ulong> runOutSectors = new();

        if(_outputPlugin is not IWritableOpticalImage outputOptical)
            return;

        // Count how many run end sectors are detected as bad blocks
        for(ulong i = blocks - 1; i > blocks - 1 - _ignoreCdrRunOuts; i--)
            if(_resume.BadBlocks.Contains(i))
                runOutSectors.Add(i);
            else
                break;

        if(runOutSectors.Count == 0)
            return;

        _dumpLog.WriteLine($"{runOutSectors.Count} sectors at the end of the disc are unreadable. This is normal in CD-R(W) discs as these sectors are created by burning software as part of the recording process. Empty ones will be generated and stored in the image.");

        UpdateStatus?.
            Invoke($"{runOutSectors.Count} sectors at the end of the disc are unreadable. This is normal in CD-R(W) discs as these sectors are created by burning software as part of the recording process. Empty ones will be generated and stored in the image.");

        foreach(ulong s in runOutSectors)
        {
            Track track = tracks.FirstOrDefault(t => t.StartSector <= s && t.EndSector >= s);

            if(track is null)
                continue;

            var sector = new byte[2352];

            switch(track.Type)
            {
                case TrackType.Audio: break;
                case TrackType.Data:
                    sector = new byte[2048];

                    break;
                case TrackType.CdMode1:         break;
                case TrackType.CdMode2Formless: break;
                case TrackType.CdMode2Form1:    break;
                case TrackType.CdMode2Form2:    break;
                default:                        continue;
            }

            if(track.Type != TrackType.Audio &&
               track.Type != TrackType.Data)
            {
                SectorBuilder sb = new();
                sb.ReconstructPrefix(ref sector, track.Type, (long)s);
                sb.ReconstructEcc(ref sector, track.Type);
            }

            if(supportsLongSectors)
                outputOptical.WriteSectorLong(sector, s);
            else
                outputOptical.WriteSector(Sector.GetUserData(sector), s);

            _resume.BadBlocks.Remove(s);
            extents.Add(s);

            if(desiredSubchannel == MmcSubchannel.None)
                continue;

            // Hidden track
            ulong trackStart;

            ulong pregap;

            if(track.Sequence == 0)
            {
                track      = tracks.FirstOrDefault(t => (int)t.Sequence == 1);
                trackStart = 0;
                pregap     = track?.StartSector ?? 0;
            }
            else
            {
                trackStart = track.StartSector;
                pregap     = track.Pregap;
            }

            byte flags;

            if(!trackFlags.TryGetValue((byte)(track?.Sequence ?? 0), out byte trkFlags) &&
               track?.Type != TrackType.Audio)
                flags = (byte)CdFlags.DataTrack;
            else
                flags = trkFlags;

            byte index;

            if(track?.Indexes?.Count > 0)
                index = (byte)track.Indexes.LastOrDefault(i => i.Value >= (int)s).Key;
            else
                index = 0;

            byte[] sub = Subchannel.Generate((int)s, track?.Sequence ?? 0, (int)pregap, (int)trackStart, flags, index);

            outputOptical.WriteSectorsTag(sub, s, 1, SectorTagType.CdSectorSubchannel);

            subLog?.WriteEntry(sub, true, (long)s, 1, true, false);
            subchannelExtents.Remove((int)s);
        }
    }
}