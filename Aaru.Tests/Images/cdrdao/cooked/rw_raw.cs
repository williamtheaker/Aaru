﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : nosub.cs
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

namespace Aaru.Tests.Images.cdrdao.cooked;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Interfaces;
using Aaru.DiscImages;
using NUnit.Framework;

[TestFixture]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class rw_raw : OpticalMediaImageTest
{
    public override string DataFolder =>
        Path.Combine(Consts.TEST_FILES_ROOT, "Media image formats", "cdrdao", "cooked", "rw_raw");
    public override IMediaImage Plugin => new Cdrdao();

    public override OpticalImageTestExpected[] Tests => new[]
    {
        new OpticalImageTestExpected
        {
            TestFile      = "report_audiocd.toc",
            MediaType     = MediaType.CDDA,
            Sectors       = 247073,
            SectorSize    = 2352,
            Md5           = "c09f408a4416634d8ac1c1ffd0ed75a5",
            LongMd5       = "c09f408a4416634d8ac1c1ffd0ed75a5",
            SubchannelMd5 = "e68088c7f1236c2304778e976057bd6e",
            Tracks = new[]
            {
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 0,
                    End     = 16548,
                    Pregap  = 150,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 16549,
                    End     = 30050,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 30051,
                    End     = 47949,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 47950,
                    End     = 63313,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 63314,
                    End     = 78924,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 78925,
                    End     = 94731,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 94732,
                    End     = 117124,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 117125,
                    End     = 136165,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 136166,
                    End     = 154071,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 154072,
                    End     = 170750,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 170751,
                    End     = 186538,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 186539,
                    End     = 201798,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 201799,
                    End     = 224448,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 224449,
                    End     = 247072,
                    Pregap  = 0,
                    Flags   = 0
                }
            }
        },
        new OpticalImageTestExpected
        {
            TestFile      = "report_cdr.toc",
            MediaType     = MediaType.CDROM,
            Sectors       = 254265,
            SectorSize    = 2048,
            Md5           = "bf4bbec517101d0d6f45d2e4d50cb875",
            LongMd5       = "3d3f9cf7d1ba2249b1e7960071e5af46",
            SubchannelMd5 = "8ed5c39d7435aeac23e77478625f14f1",
            Tracks = new[]
            {
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 0,
                    End     = 254264,
                    Pregap  = 150,
                    Flags   = 4,
                    Number  = 1,
                    FileSystems = new[]
                    {
                        new FileSystemTest
                        {
                            Clusters    = 63562,
                            ClusterSize = 8192,
                            Type        = "HFS",
                            VolumeName  = "Winpower"
                        },
                        new FileSystemTest
                        {
                            Clusters    = 254265,
                            ClusterSize = 2048,
                            Type        = "ISO9660",
                            VolumeName  = "Winpower"
                        }
                    }
                }
            }
        },
        new OpticalImageTestExpected
        {
            TestFile      = "report_cdrom.toc",
            MediaType     = MediaType.CDROM,
            Sectors       = 254265,
            SectorSize    = 2048,
            Md5           = "bf4bbec517101d0d6f45d2e4d50cb875",
            LongMd5       = "3d3f9cf7d1ba2249b1e7960071e5af46",
            SubchannelMd5 = "0ba3fa223e887fb955458fc7b40b0396",
            Tracks = new[]
            {
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 0,
                    End     = 254264,
                    Pregap  = 150,
                    Flags   = 4,
                    Number  = 1,
                    FileSystems = new[]
                    {
                        new FileSystemTest
                        {
                            Clusters    = 63562,
                            ClusterSize = 8192,
                            Type        = "HFS",
                            VolumeName  = "Winpower"
                        },
                        new FileSystemTest
                        {
                            Clusters    = 254265,
                            ClusterSize = 2048,
                            Type        = "ISO9660",
                            VolumeName  = "Winpower"
                        }
                    }
                }
            }
        },
        new OpticalImageTestExpected
        {
            TestFile      = "report_cdrw.toc",
            MediaType     = MediaType.CDROM,
            Sectors       = 308224,
            SectorSize    = 2048,
            Md5           = "1e55aa420ca8f8ea77d5b597c9cfc19b",
            LongMd5       = "3af5f943ddb9427d9c63a4ce3b704db9",
            SubchannelMd5 = "eaf6dcfb86e9aca9856d736f3b04c765",
            Tracks = new[]
            {
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 0,
                    End     = 308223,
                    Pregap  = 150,
                    Flags   = 4,
                    Number  = 1,
                    FileSystems = new[]
                    {
                        new FileSystemTest
                        {
                            Bootable    = true,
                            Clusters    = 308224,
                            ClusterSize = 2048,
                            Type        = "ISO9660",
                            VolumeName  = "ARCH_201901"
                        }
                    }
                }
            }
        },
        new OpticalImageTestExpected
        {
            TestFile      = "test_multi_karaoke_sampler.toc",
            MediaType     = MediaType.CDROM,
            Sectors       = 329008,
            SectorSize    = 2048,
            Md5           = "f09312ba25a479fb81912a2965babd22",
            LongMd5       = "f48603d11883593f45ec4a3824681e4e",
            SubchannelMd5 = "7ac088d8a737d8d39b1cced86d11c7b5",
            Tracks = new[]
            {
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 0,
                    End     = 1736,
                    Pregap  = 150,
                    Flags   = 4,
                    Number  = 1,
                    FileSystems = new[]
                    {
                        new FileSystemTest
                        {
                            Clusters    = 1587,
                            ClusterSize = 2048,
                            Type        = "ISO9660",
                            VolumeName  = ""
                        }
                    }
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 1737,
                    End     = 32598,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 32599,
                    End     = 52521,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 52522,
                    End     = 70153,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 70154,
                    End     = 99947,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 99948,
                    End     = 119610,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 119611,
                    End     = 136848,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 136849,
                    End     = 155639,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 155640,
                    End     = 175675,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 175676,
                    End     = 206310,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 206311,
                    End     = 226299,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 226300,
                    End     = 244204,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 244205,
                    End     = 273814,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 273815,
                    End     = 293601,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 293602,
                    End     = 310560,
                    Pregap  = 0,
                    Flags   = 0
                },
                new TrackInfoTestExpected
                {
                    Session = 1,
                    Start   = 310561,
                    End     = 329007,
                    Pregap  = 150,
                    Flags   = 0
                }
            }
        }
    };
}