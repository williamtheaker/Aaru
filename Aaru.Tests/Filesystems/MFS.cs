﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : MFS.cs
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

namespace Aaru.Tests.Filesystems;

using System.IO;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Interfaces;
using Aaru.Filesystems;
using NUnit.Framework;

[TestFixture]
public class Mfs : ReadOnlyFilesystemTest
{
    public Mfs() : base("MFS") {}

    public override string DataFolder => Path.Combine(Consts.TEST_FILES_ROOT, "Filesystems", "Macintosh File System");
    public override IFilesystem Plugin => new AppleMFS();
    public override bool Partitions => false;

    public override FileSystemTest[] Tests => new[]
    {
        new FileSystemTest
        {
            TestFile    = "macos_0.1_mf1dd.img.lz",
            MediaType   = MediaType.AppleSonySS,
            Sectors     = 800,
            SectorSize  = 512,
            Clusters    = 391,
            ClusterSize = 1024,
            VolumeName  = "Volume label"
        },
        new FileSystemTest
        {
            TestFile    = "macos_0.5_mf1dd.img.lz",
            MediaType   = MediaType.AppleSonySS,
            Sectors     = 800,
            SectorSize  = 512,
            Clusters    = 391,
            ClusterSize = 1024,
            VolumeName  = "Volume label"
        },
        new FileSystemTest
        {
            TestFile    = "macos_1.1_mf1dd.img.lz",
            MediaType   = MediaType.AppleSonySS,
            Sectors     = 800,
            SectorSize  = 512,
            Clusters    = 391,
            ClusterSize = 1024,
            VolumeName  = "Volume label"
        },
        new FileSystemTest
        {
            TestFile    = "macos_2.0_mf1dd.img.lz",
            MediaType   = MediaType.AppleSonySS,
            Sectors     = 800,
            SectorSize  = 512,
            Clusters    = 391,
            ClusterSize = 1024,
            VolumeName  = "Volume label"
        },
        new FileSystemTest
        {
            TestFile    = "macos_6.0.7_mf1dd.img.lz",
            MediaType   = MediaType.AppleSonySS,
            Sectors     = 800,
            SectorSize  = 512,
            Clusters    = 391,
            ClusterSize = 1024,
            VolumeName  = "Volume label"
        }
    };
}