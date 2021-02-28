﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : TeleDisk.cs
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
// Copyright © 2011-2021 Natalia Portillo
// ****************************************************************************/

using System;
using System.IO;
using Aaru.Checksums;
using Aaru.CommonTypes;
using Aaru.Filters;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace Aaru.Tests.Images.pce
{
    [TestFixture]
    public class TeleDisk
    {
        readonly string[] _testFiles =
        {
            "md1dd_8.td0.lz", "md1dd.td0.lz", "md2dd_8.td0.lz", "md2dd.td0.lz", "md2hd_nec.td0.lz", "md2hd.td0.lz",
            "mf1dd_10.td0.lz", "mf1dd_11.td0.lz", "mf2dd_10.td0.lz", "mf2dd_11.td0.lz", "mf2dd_fdformat_800.td0.lz",
            "mf2dd_fdformat_820.td0.lz", "mf2dd_freedos.td0.lz", "mf2dd.td0.lz", "mf2ed.td0.lz", "mf2hd_2m_max.td0.lz",
            "mf2hd_2m.td0.lz", "mf2hd_fdformat_168.td0.lz", "mf2hd_fdformat_172.td0.lz", "mf2hd_freedos.td0.lz",
            "mf2hd.td0.lz", "mf2hd_xdf.td0.lz", "mf2hd_xdf_teledisk.td0.lz", "rx01.td0.lz", "rx50.td0.lz"
        };
        readonly ulong[] _sectors =
        {
            // md1dd_8.td0.lz
            320,

            // md1dd.td0.lz
            360,

            // md2dd_8.td0.lz
            640,

            // md2dd.td0.lz
            720,

            // md2hd_nec.td0.lz
            1232,

            // md2hd.td0.lz
            2400,

            // mf1dd_10.td0.lz
            800,

            // mf1dd_11.td0.lz
            880,

            // mf2dd_10.td0.lz
            1600,

            // mf2dd_11.td0.lz
            1760,

            // mf2dd_fdformat_800.td0.lz
            1600,

            // mf2dd_fdformat_820.td0.lz
            1640,

            // mf2dd_freedos.td0.lz
            1640,

            // mf2dd.td0.lz
            1440,

            // mf2ed.td0.lz
            5760,

            // mf2hd_2m_max.td0.lz
            1148,

            // mf2hd_2m.td0.lz
            1804,

            // mf2hd_fdformat_168.td0.lz
            332,

            // mf2hd_fdformat_172.td0.lz
            3444,

            // mf2hd_freedos.td0.lz
            3486,

            // mf2hd.td0.lz
            2880,

            // mf2hd_xdf.td0.lz
            640,

            // mf2hd_xdf_teledisk.td0.lz
            640,

            // rx01.td0.lz
            2002,

            // rx50.td0.lz
            800
        };
        readonly uint[] _sectorSize =
        {
            // md1dd_8.td0.lz
            512,

            // md1dd.td0.lz
            512,

            // md2dd_8.td0.lz
            512,

            // md2dd.td0.lz
            512,

            // md2hd_nec.td0.lz
            1024,

            // md2hd.td0.lz
            512,

            // mf1dd_10.td0.lz
            512,

            // mf1dd_11.td0.lz
            512,

            // mf2dd_10.td0.lz
            512,

            // mf2dd_11.td0.lz
            512,

            // mf2dd_fdformat_800.td0.lz
            512,

            // mf2dd_fdformat_820.td0.lz
            512,

            // mf2dd_freedos.td0.lz
            512,

            // mf2dd.td0.lz
            512,

            // mf2ed.td0.lz
            512,

            // mf2hd_2m_max.td0.lz
            512,

            // mf2hd_2m.td0.lz
            512,

            // mf2hd_fdformat_168.td0.lz
            512,

            // mf2hd_fdformat_172.td0.lz
            512,

            // mf2hd_freedos.td0.lz
            512,

            // mf2hd.td0.lz
            512,

            // mf2hd_xdf.td0.lz
            512,

            // mf2hd_xdf_teledisk.td0.lz
            512,

            // rx01.td0.lz
            0,

            // rx50.td0.lz
            512
        };
        readonly MediaType[] _mediaTypes =
        {
            // md1dd_8.td0.lz
            MediaType.DOS_525_SS_DD_8,

            // md1dd.td0.lz
            MediaType.DOS_525_SS_DD_9,

            // md2dd_8.td0.lz
            MediaType.DOS_525_DS_DD_8,

            // md2dd.td0.lz
            MediaType.DOS_525_DS_DD_9,

            // md2hd_nec.td0.lz
            MediaType.NEC_35_HD_8,

            // md2hd.td0.lz
            MediaType.DOS_525_HD,

            // mf1dd_10.td0.lz
            MediaType.Unknown,

            // mf1dd_11.td0.lz
            MediaType.Unknown,

            // mf2dd_10.td0.lz
            MediaType.CBM_35_DD,

            // mf2dd_11.td0.lz
            MediaType.CBM_AMIGA_35_DD,

            // mf2dd_fdformat_800.td0.lz
            MediaType.CBM_35_DD,

            // mf2dd_fdformat_820.td0.lz
            MediaType.Unknown,

            // mf2dd_freedos.td0.lz
            MediaType.Unknown,

            // mf2dd.td0.lz
            MediaType.DOS_35_DS_DD_9,

            // mf2ed.td0.lz
            MediaType.ECMA_147,

            // mf2hd_2m_max.td0.lz
            MediaType.Unknown,

            // mf2hd_2m.td0.lz
            MediaType.Unknown,

            // mf2hd_fdformat_168.td0.lz
            MediaType.Unknown,

            // mf2hd_fdformat_172.td0.lz
            MediaType.Unknown,

            // mf2hd_freedos.td0.lz
            MediaType.Unknown,

            // mf2hd.td0.lz
            MediaType.Unknown,

            // mf2hd_xdf.td0.lz
            MediaType.Unknown,

            // mf2hd_xdf_teledisk.td0.lz
            MediaType.XDF_35,

            // rx01.td0.lz
            MediaType.Unknown,

            // rx50.td0.lz
            MediaType.Unknown
        };
        readonly string[] _md5S =
        {
            // md1dd_8.td0.lz
            "8308e749af855a3ded48d474eb7c305e",

            // md1dd.td0.lz
            "b7b8a69b10ee4ec921aa8eea232fdd75",

            // md2dd_8.td0.lz
            "f4a77a2d2a1868dc18e8b92032d02fd2",

            // md2dd.td0.lz
            "099d95ac42d1a8010f914ac64ede7a70",

            // md2hd_nec.td0.lz
            "fd54916f713d01b670c1a5df5e74a97f",

            // md2hd.td0.lz
            "3df7cd10044af75d77e8936af0dbf9ff",

            // mf1dd_10.td0.lz
            "d75d3e79d9c5051922d4c2226fa4a6ff",

            // mf1dd_11.td0.lz
            "e16ed33a1a466826562c681d8bdf3e27",

            // mf2dd_10.td0.lz
            "fd48b2c12097cbc646b4a93ef4f92259",

            // mf2dd_11.td0.lz
            "512f7175e753e2e2ad620d448c42545d",

            // mf2dd_fdformat_800.td0.lz
            "c533488a21098a62c85f1649abda2803",

            // mf2dd_fdformat_820.td0.lz
            "db9cfb6eea18820b7a7e0b5b45594471",

            // mf2dd_freedos.td0.lz
            "456390a9c6ab05cb458a03c47296de08",

            // mf2dd.td0.lz
            "de3f85896f771b7e5bc4c9e3926d64e4",

            // mf2ed.td0.lz
            "854d0d49a522b64af698e319a24cd68e",

            // mf2hd_2m_max.td0.lz
            "4b88a3e43b57778422e8b1e851a9c902",

            // mf2hd_2m.td0.lz
            "d032d928c43b66419b7404b016ec07ff",

            // mf2hd_fdformat_168.td0.lz
            "62b900808c3e9f91f8361fd1716155a1",

            // mf2hd_fdformat_172.td0.lz
            "9dea1e119a73a21a38d134f36b2e5564",

            // mf2hd_freedos.td0.lz
            "dbd52e9e684f97d9e2292811242bb24e",

            // mf2hd.td0.lz
            "b4a602f67903c46eef62addb0780aa56",

            // mf2hd_xdf.td0.lz
            "57965378275db490527ff8c8fc517adf",

            // mf2hd_xdf_teledisk.td0.lz
            "728f9361203dc39961b1413aa050f70d",

            // rx01.td0.lz
            "UNKNOWN",

            // rx50.td0.lz
            "ccd4431139755c58f340681f63510642"
        };

        readonly string _dataFolder = Path.Combine(Consts.TEST_FILES_ROOT, "Media image formats", "pce", "TeleDisk");

        [Test]
        public void Info()
        {
            Environment.CurrentDirectory = _dataFolder;

            Assert.Multiple(() =>
            {
                for(int i = 0; i < _testFiles.Length; i++)
                {
                    var filter = new LZip();
                    filter.Open(_testFiles[i]);

                    var  image  = new DiscImages.TeleDisk();
                    bool opened = image.Open(filter);

                    Assert.AreEqual(true, opened, $"Open: {_testFiles[i]}");

                    if(!opened)
                        continue;

                    using(new AssertionScope())
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.AreEqual(_sectors[i], image.Info.Sectors, $"Sectors: {_testFiles[i]}");
                            Assert.AreEqual(_sectorSize[i], image.Info.SectorSize, $"Sector size: {_testFiles[i]}");
                            Assert.AreEqual(_mediaTypes[i], image.Info.MediaType, $"Media type: {_testFiles[i]}");
                        });
                    }
                }
            });
        }

        // How many sectors to read at once
        const uint SECTORS_TO_READ = 256;

        [Test]
        public void Hashes()
        {
            Environment.CurrentDirectory = _dataFolder;

            Assert.Multiple(() =>
            {
                for(int i = 0; i < _testFiles.Length; i++)
                {
                    var filter = new LZip();
                    filter.Open(_testFiles[i]);

                    var   image       = new DiscImages.TeleDisk();
                    bool  opened      = image.Open(filter);
                    ulong doneSectors = 0;

                    Assert.AreEqual(true, opened, $"Open: {_testFiles[i]}");

                    if(!opened)
                        continue;

                    var ctx = new Md5Context();

                    while(doneSectors < image.Info.Sectors)
                    {
                        byte[] sector;

                        if(image.Info.Sectors - doneSectors >= SECTORS_TO_READ)
                        {
                            sector      =  image.ReadSectors(doneSectors, SECTORS_TO_READ);
                            doneSectors += SECTORS_TO_READ;
                        }
                        else
                        {
                            sector      =  image.ReadSectors(doneSectors, (uint)(image.Info.Sectors - doneSectors));
                            doneSectors += image.Info.Sectors - doneSectors;
                        }

                        ctx.Update(sector);
                    }

                    Assert.AreEqual(_md5S[i], ctx.End(), $"Hash: {_testFiles[i]}");
                }
            });
        }
    }
}