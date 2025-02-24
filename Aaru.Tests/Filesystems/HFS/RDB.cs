﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : HFS.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Aaru unit testing.
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

namespace Aaru.Tests.Filesystems.HFS;

using System.IO;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Interfaces;
using Aaru.Filesystems;
using NUnit.Framework;

[TestFixture]
public class RDB : FilesystemTest
{
    public RDB() : base("HFS") {}

    public override string      DataFolder => Path.Combine(Consts.TEST_FILES_ROOT, "Filesystems", "Apple HFS (RDB)");
    public override IFilesystem Plugin     => new AppleHFS();
    public override bool        Partitions => true;

    public override FileSystemTest[] Tests => new[]
    {
        new FileSystemTest
        {
            TestFile    = "amigaos_3.9.aif",
            MediaType   = MediaType.GENERIC_HDD,
            Sectors     = 1024128,
            SectorSize  = 512,
            Clusters    = 63752,
            ClusterSize = 8192,
            VolumeName  = "Volume label"
        }
    };
}