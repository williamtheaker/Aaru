﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : IMD.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Manages Sydex IMD disc images.
//
// --[ License ] --------------------------------------------------------------
//
//     This library is free software; you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as
//     published by the Free Software Foundation; either version 2.1 of the
//     License, or (at your option) any later version.
//
//     This library is distributed in the hope that it will be useful, but
//     WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//     Lesser General Public License for more details.
//
//     You should have received a copy of the GNU Lesser General Public
//     License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2018 Natalia Portillo
// ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DiscImageChef.CommonTypes;
using DiscImageChef.Console;
using DiscImageChef.Filters;

namespace DiscImageChef.DiscImages
{
    public class Imd : IMediaImage
    {
        const byte SECTOR_CYLINDER_MAP_MASK = 0x80;
        const byte SECTOR_HEAD_MAP_MASK = 0x40;
        const byte COMMENT_END = 0x1A;
        const string REGEX_HEADER =
                @"IMD (?<version>\d.\d+):\s+(?<day>\d+)\/\s*(?<month>\d+)\/(?<year>\d+)\s+(?<hour>\d+):(?<minute>\d+):(?<second>\d+)\r\n"
            ;
        ImageInfo imageInfo;

        List<byte[]> sectorsData;

        public Imd()
        {
            imageInfo = new ImageInfo
            {
                ReadableSectorTags = new List<SectorTagType>(),
                ReadableMediaTags = new List<MediaTagType>(),
                HasPartitions = false,
                HasSessions = false,
                Version = null,
                Application = null,
                ApplicationVersion = null,
                Creator = null,
                Comments = null,
                MediaManufacturer = null,
                MediaModel = null,
                MediaSerialNumber = null,
                MediaBarcode = null,
                MediaPartNumber = null,
                MediaSequence = 0,
                LastMediaSequence = 0,
                DriveManufacturer = null,
                DriveModel = null,
                DriveSerialNumber = null,
                DriveFirmwareRevision = null
            };
        }

        public string Name => "Dunfield's IMD";
        public Guid Id => new Guid("0D67162E-38A3-407D-9B1A-CF40080A48CB");
        public ImageInfo Info => imageInfo;

        public string ImageFormat => "IMageDisk";

        public List<Partition> Partitions =>
            throw new FeatureUnsupportedImageException("Feature not supported by image format");

        public List<Track> Tracks =>
            throw new FeatureUnsupportedImageException("Feature not supported by image format");

        public List<Session> Sessions =>
            throw new FeatureUnsupportedImageException("Feature not supported by image format");

        public bool IdentifyImage(IFilter imageFilter)
        {
            Stream stream = imageFilter.GetDataForkStream();
            stream.Seek(0, SeekOrigin.Begin);
            if(stream.Length < 31) return false;

            byte[] hdr = new byte[31];
            stream.Read(hdr, 0, 31);

            Regex hr = new Regex(REGEX_HEADER);
            Match hm = hr.Match(Encoding.ASCII.GetString(hdr));

            return hm.Success;
        }

        public bool OpenImage(IFilter imageFilter)
        {
            Stream stream = imageFilter.GetDataForkStream();
            stream.Seek(0, SeekOrigin.Begin);

            MemoryStream cmt = new MemoryStream();
            stream.Seek(0x1F, SeekOrigin.Begin);
            for(uint i = 0; i < stream.Length; i++)
            {
                byte b = (byte)stream.ReadByte();
                if(b == 0x1A) break;

                cmt.WriteByte(b);
            }

            imageInfo.Comments = StringHandlers.CToString(cmt.ToArray());
            sectorsData = new List<byte[]>();

            byte currentCylinder = 0;
            imageInfo.Cylinders = 1;
            imageInfo.Heads = 1;
            ulong currentLba = 0;

            TransferRate mode = TransferRate.TwoHundred;

            while(stream.Position + 5 < stream.Length)
            {
                mode = (TransferRate)stream.ReadByte();
                byte cylinder = (byte)stream.ReadByte();
                byte head = (byte)stream.ReadByte();
                byte spt = (byte)stream.ReadByte();
                byte n = (byte)stream.ReadByte();
                byte[] idmap = new byte[spt];
                byte[] cylmap = new byte[spt];
                byte[] headmap = new byte[spt];
                ushort[] bps = new ushort[spt];

                if(cylinder != currentCylinder)
                {
                    currentCylinder = cylinder;
                    imageInfo.Cylinders++;
                }

                if((head & 1) == 1) imageInfo.Heads = 2;

                stream.Read(idmap, 0, idmap.Length);
                if((head & SECTOR_CYLINDER_MAP_MASK) == SECTOR_CYLINDER_MAP_MASK) stream.Read(cylmap, 0, cylmap.Length);
                if((head & SECTOR_HEAD_MAP_MASK) == SECTOR_HEAD_MAP_MASK) stream.Read(headmap, 0, headmap.Length);
                if(n == 0xFF)
                {
                    byte[] bpsbytes = new byte[spt * 2];
                    stream.Read(bpsbytes, 0, bpsbytes.Length);
                    for(int i = 0; i < spt; i++) bps[i] = BitConverter.ToUInt16(bpsbytes, i * 2);
                }
                else for(int i = 0; i < spt; i++) bps[i] = (ushort)(128 << n);

                if(spt > imageInfo.SectorsPerTrack) imageInfo.SectorsPerTrack = spt;

                SortedDictionary<byte, byte[]> track = new SortedDictionary<byte, byte[]>();

                for(int i = 0; i < spt; i++)
                {
                    SectorType type = (SectorType)stream.ReadByte();
                    byte[] data = new byte[bps[i]];

                    // TODO; Handle disks with different bps in track 0
                    if(bps[i] > imageInfo.SectorSize) imageInfo.SectorSize = bps[i];

                    switch(type)
                    {
                        case SectorType.Unavailable:
                            if(!track.ContainsKey(idmap[i])) track.Add(idmap[i], data);
                            break;
                        case SectorType.Normal:
                        case SectorType.Deleted:
                        case SectorType.Error:
                        case SectorType.DeletedError:
                            stream.Read(data, 0, data.Length);
                            if(!track.ContainsKey(idmap[i])) track.Add(idmap[i], data);
                            imageInfo.ImageSize += (ulong)data.Length;
                            break;
                        case SectorType.Compressed:
                        case SectorType.CompressedDeleted:
                        case SectorType.CompressedError:
                        case SectorType.CompressedDeletedError:
                            byte filling = (byte)stream.ReadByte();
                            ArrayHelpers.ArrayFill(data, filling);
                            if(!track.ContainsKey(idmap[i])) track.Add(idmap[i], data);
                            break;
                        default: throw new ImageNotSupportedException($"Invalid sector type {(byte)type}");
                    }
                }

                foreach(KeyValuePair<byte, byte[]> kvp in track)
                {
                    sectorsData.Add(kvp.Value);
                    currentLba++;
                }
            }

            imageInfo.Application = "IMD";
            // TODO: The header is the date of dump or the date of the application compilation?
            imageInfo.CreationTime = imageFilter.GetCreationTime();
            imageInfo.LastModificationTime = imageFilter.GetLastWriteTime();
            imageInfo.MediaTitle = Path.GetFileNameWithoutExtension(imageFilter.GetFilename());
            imageInfo.Comments = StringHandlers.CToString(cmt.ToArray());
            imageInfo.Sectors = currentLba;
            imageInfo.MediaType = MediaType.Unknown;

            switch(mode)
            {
                case TransferRate.TwoHundred:
                case TransferRate.ThreeHundred:
                    if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 10 &&
                       imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ACORN_525_SS_SD_40;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ACORN_525_SS_SD_80;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 18 &&
                            imageInfo.SectorSize == 128) imageInfo.MediaType = MediaType.ATARI_525_SD;
                    break;
                case TransferRate.FiveHundred:
                    if(imageInfo.Heads == 1 && imageInfo.Cylinders == 32 && imageInfo.SectorsPerTrack == 8 &&
                       imageInfo.SectorSize == 319) imageInfo.MediaType = MediaType.IBM23FD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 73 && imageInfo.SectorsPerTrack == 26 &&
                            imageInfo.SectorSize == 128) imageInfo.MediaType = MediaType.IBM23FD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 77 && imageInfo.SectorsPerTrack == 26 &&
                            imageInfo.SectorSize == 128) imageInfo.MediaType = MediaType.NEC_8_SD;
                    break;
                case TransferRate.TwoHundredMfm:
                case TransferRate.ThreeHundredMfm:
                    if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 8 &&
                       imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_525_SS_DD_8;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 9 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_525_SS_DD_9;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 8 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_525_DS_DD_8;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 9 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_525_DS_DD_9;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 18 &&
                            imageInfo.SectorSize == 128) imageInfo.MediaType = MediaType.ATARI_525_SD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 26 &&
                            imageInfo.SectorSize == 128) imageInfo.MediaType = MediaType.ATARI_525_ED;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 18 &&
                            imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ATARI_525_DD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 16 &&
                            imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ACORN_525_SS_DD_40;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 16 &&
                            imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ACORN_525_SS_DD_80;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 40 && imageInfo.SectorsPerTrack == 18 &&
                            imageInfo.SectorSize == 256) imageInfo.MediaType = MediaType.ATARI_525_DD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.RX50;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 9 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_35_DS_DD_9;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 8 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_35_DS_DD_8;
                    if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 9 &&
                       imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_35_SS_DD_9;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 8 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_35_SS_DD_8;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 5 &&
                            imageInfo.SectorSize == 1024) imageInfo.MediaType = MediaType.ACORN_35_DS_DD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 82 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.FDFORMAT_35_DD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 70 && imageInfo.SectorsPerTrack == 9 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.Apricot_35;
                    break;
                case TransferRate.FiveHundredMfm:
                    if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 18 &&
                       imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_35_HD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 21 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DMF;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 82 && imageInfo.SectorsPerTrack == 21 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DMF_82;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 23 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.XDF_35;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 15 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.DOS_525_HD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 1024) imageInfo.MediaType = MediaType.ACORN_35_DS_HD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 77 && imageInfo.SectorsPerTrack == 8 &&
                            imageInfo.SectorSize == 1024) imageInfo.MediaType = MediaType.NEC_525_HD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 9 &&
                            imageInfo.SectorSize == 1024) imageInfo.MediaType = MediaType.SHARP_525_9;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.ATARI_35_SS_DD;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 10 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.ATARI_35_DS_DD;
                    else if(imageInfo.Heads == 1 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 11 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.ATARI_35_SS_DD_11;
                    else if(imageInfo.Heads == 2 && imageInfo.Cylinders == 80 && imageInfo.SectorsPerTrack == 11 &&
                            imageInfo.SectorSize == 512) imageInfo.MediaType = MediaType.ATARI_35_DS_DD_11;
                    break;
                default:
                    imageInfo.MediaType = MediaType.Unknown;
                    break;
            }

            imageInfo.XmlMediaType = XmlMediaType.BlockMedia;

            DicConsole.VerboseWriteLine("IMD image contains a disk of type {0}", imageInfo.MediaType);
            if(!string.IsNullOrEmpty(imageInfo.Comments))
                DicConsole.VerboseWriteLine("IMD comments: {0}", imageInfo.Comments);

            /*
            FileStream debugFs = new FileStream("debug.img", FileMode.CreateNew, FileAccess.Write);
            for(ulong i = 0; i < ImageInfo.sectors; i++)
                debugFs.Write(ReadSector(i), 0, (int)ImageInfo.sectorSize);
            debugFs.Dispose();
            */

            return true;
        }

        public bool? VerifySector(ulong sectorAddress)
        {
            return null;
        }

        public bool? VerifySector(ulong sectorAddress, uint track)
        {
            return null;
        }

        public bool? VerifySectors(ulong sectorAddress, uint length, out List<ulong> failingLbas,
                                            out List<ulong> unknownLbas)
        {
            failingLbas = new List<ulong>();
            unknownLbas = new List<ulong>();

            for(ulong i = sectorAddress; i < sectorAddress + length; i++) unknownLbas.Add(i);

            return null;
        }

        public bool? VerifySectors(ulong sectorAddress, uint length, uint track, out List<ulong> failingLbas,
                                            out List<ulong> unknownLbas)
        {
            failingLbas = new List<ulong>();
            unknownLbas = new List<ulong>();

            for(ulong i = sectorAddress; i < sectorAddress + length; i++) unknownLbas.Add(i);

            return null;
        }

        public bool? VerifyMediaImage()
        {
            return null;
        }

        public byte[] ReadSector(ulong sectorAddress)
        {
            return ReadSectors(sectorAddress, 1);
        }

        public byte[] ReadSectors(ulong sectorAddress, uint length)
        {
            if(sectorAddress > imageInfo.Sectors - 1)
                throw new ArgumentOutOfRangeException(nameof(sectorAddress), "Sector address not found");

            if(sectorAddress + length > imageInfo.Sectors)
                throw new ArgumentOutOfRangeException(nameof(length), "Requested more sectors than available");

            MemoryStream buffer = new MemoryStream();
            for(int i = 0; i < length; i++)
                buffer.Write(sectorsData[(int)sectorAddress + i], 0, sectorsData[(int)sectorAddress + i].Length);

            return buffer.ToArray();
        }

        public byte[] ReadSectorTag(ulong sectorAddress, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorsTag(ulong sectorAddress, uint length, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorLong(ulong sectorAddress)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorsLong(ulong sectorAddress, uint length)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadDiskTag(MediaTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public List<Track> GetSessionTracks(Session session)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public List<Track> GetSessionTracks(ushort session)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSector(ulong sectorAddress, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorTag(ulong sectorAddress, uint track, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectors(ulong sectorAddress, uint length, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorsTag(ulong sectorAddress, uint length, uint track, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorLong(ulong sectorAddress, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public byte[] ReadSectorsLong(ulong sectorAddress, uint length, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        enum TransferRate : byte
        {
            /// <summary>500 kbps in FM mode</summary>
            FiveHundred = 0,
            /// <summary>300 kbps in FM mode</summary>
            ThreeHundred = 1,
            /// <summary>250 kbps in FM mode</summary>
            TwoHundred = 2,
            /// <summary>500 kbps in MFM mode</summary>
            FiveHundredMfm = 3,
            /// <summary>300 kbps in MFM mode</summary>
            ThreeHundredMfm = 4,
            /// <summary>250 kbps in MFM mode</summary>
            TwoHundredMfm = 5
        }

        enum SectorType : byte
        {
            Unavailable = 0,
            Normal = 1,
            Compressed = 2,
            Deleted = 3,
            CompressedDeleted = 4,
            Error = 5,
            CompressedError = 6,
            DeletedError = 7,
            CompressedDeletedError = 8
        }
    }
}