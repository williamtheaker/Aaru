// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : BLU.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Manages Basic Lisa Utility disk images.
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
// Copyright © 2011-2022 Natalia Portillo
// ****************************************************************************/

namespace Aaru.DiscImages;

using System.Collections.Generic;
using System.IO;
using Aaru.CommonTypes.Enums;
using Aaru.CommonTypes.Interfaces;
using Aaru.CommonTypes.Structs;

/// <inheritdoc cref="Aaru.CommonTypes.Interfaces.IWritableImage" />
/// <summary>Implements reading Basic Lisa Utility disk images</summary>
public sealed partial class Blu : IWritableImage, IVerifiableSectorsImage
{
    IFilter    _bluImageFilter;
    int        _bptag;
    BluHeader  _imageHeader;
    ImageInfo  _imageInfo;
    FileStream _writingStream;

    public Blu() => _imageInfo = new ImageInfo
    {
        ReadableSectorTags    = new List<SectorTagType>(),
        ReadableMediaTags     = new List<MediaTagType>(),
        HasPartitions         = false,
        HasSessions           = false,
        Version               = null,
        Application           = null,
        ApplicationVersion    = null,
        Creator               = null,
        Comments              = null,
        MediaManufacturer     = null,
        MediaModel            = null,
        MediaSerialNumber     = null,
        MediaBarcode          = null,
        MediaPartNumber       = null,
        MediaSequence         = 0,
        LastMediaSequence     = 0,
        DriveManufacturer     = null,
        DriveModel            = null,
        DriveSerialNumber     = null,
        DriveFirmwareRevision = null
    };

    struct BluHeader
    {
        public byte[] DeviceName;
        public uint   DeviceType;
        public uint   DeviceBlocks;
        public ushort BytesPerBlock;
    }
}