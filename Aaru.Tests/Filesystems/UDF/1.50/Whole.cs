﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : UDF.cs
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
// ReSharper disable CheckNamespace

namespace Aaru.Tests.Filesystems.UDF._150;

using System.IO;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Interfaces;
using Aaru.Filesystems;
using NUnit.Framework;

[TestFixture]
public class Whole : FilesystemTest
{
    public override string DataFolder =>
        Path.Combine(Consts.TEST_FILES_ROOT, "Filesystems", "Universal Disc Format", "1.50");
    public override IFilesystem Plugin     => new UDF();
    public override bool        Partitions => false;

    public override FileSystemTest[] Tests => new[]
    {
        new FileSystemTest
        {
            TestFile     = "linux.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 1024000,
            SectorSize   = 512,
            Clusters     = 1024000,
            ClusterSize  = 512,
            SystemId     = "*Linux UDFFS",
            Type         = "UDF v1.50",
            VolumeName   = "Volume label",
            VolumeSerial = "595c5d00c5b3405aLinuxUDF"
        },
        new FileSystemTest
        {
            TestFile     = "macosx_10.11.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 409600,
            SectorSize   = 512,
            Clusters     = 409600,
            ClusterSize  = 512,
            SystemId     = "*Apple Mac OS X UDF FS",
            Type         = "UDF v1.50",
            VolumeName   = "Volume label",
            VolumeSerial = "4DD0458B (Mac OS X newfs_udf) UDF Volume Set"
        },
        new FileSystemTest
        {
            TestFile     = "solaris_9.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 8388608,
            SectorSize   = 512,
            Clusters     = 8388608,
            ClusterSize  = 512,
            SystemId     = "*SUN SOLARIS UDF",
            Type         = "UDF v1.50",
            VolumeName   = "*NoLabel*",
            VolumeSerial = "595EB55A"
        },
        new FileSystemTest
        {
            TestFile     = "linux_4.19_udf_1.50_flashdrive.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 1024000,
            SectorSize   = 512,
            Clusters     = 1024000,
            ClusterSize  = 512,
            SystemId     = "*Linux UDFFS",
            Type         = "UDF v2.01",
            VolumeName   = "DicSetter",
            VolumeSerial = "5cc78f8bba4dfe00LinuxUDF"
        },
        new FileSystemTest
        {
            TestFile     = "netbsd_6.1.5.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 262144,
            SectorSize   = 512,
            Clusters     = 262144,
            ClusterSize  = 512,
            SystemId     = "*NetBSD userland UDF",
            Type         = "UDF v1.50",
            VolumeName   = "anonymous",
            VolumeSerial = "441072592d72c6e9"
        },
        new FileSystemTest
        {
            TestFile     = "netbsd_7.1.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 262144,
            SectorSize   = 512,
            Clusters     = 262144,
            ClusterSize  = 512,
            SystemId     = "*NetBSD userland UDF",
            Type         = "UDF v1.50",
            VolumeName   = "anonymous",
            VolumeSerial = "5b2ab9f9605af1ae"
        },
        new FileSystemTest
        {
            TestFile     = "appleudf_1.5.3.aif",
            MediaType    = MediaType.GENERIC_HDD,
            Sectors      = 262144,
            SectorSize   = 512,
            Clusters     = 65536,
            ClusterSize  = 2048,
            SystemId     = "*Apple Computer, Inc.",
            Type         = "UDF v1.50",
            VolumeName   = "Volume label",
            VolumeSerial = "DCC41202AppleUDF"
        }
    };
}