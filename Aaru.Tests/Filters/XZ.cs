﻿// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : XZ.cs
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

namespace Aaru.Tests.Filters;

using System.IO;
using Aaru.Checksums;
using Aaru.CommonTypes.Enums;
using Aaru.CommonTypes.Interfaces;
using Aaru.Filters;
using NUnit.Framework;

[TestFixture]
public class Xz
{
    static readonly byte[] _expectedFile =
    {
        0x6c, 0x88, 0xa5, 0x9a, 0x1b, 0x7a, 0xec, 0x59, 0x2b, 0xef, 0x8a, 0x28, 0xdb, 0x11, 0x01, 0xc8
    };
    static readonly byte[] _expectedContents =
    {
        0x18, 0x90, 0x5a, 0xf9, 0x83, 0xd8, 0x2b, 0xdd, 0x1a, 0xcc, 0x69, 0x75, 0x4f, 0x0f, 0x81, 0x5e
    };
    readonly string _location;

    public Xz() => _location = Path.Combine(Consts.TEST_FILES_ROOT, "Filters", "xz.xz");

    [Test]
    public void CheckContents()
    {
        IFilter filter = new XZ();
        filter.Open(_location);
        Stream str  = filter.GetDataForkStream();
        var    data = new byte[1048576];
        str.Read(data, 0, 1048576);
        str.Close();
        str.Dispose();
        filter.Close();
        Md5Context.Data(data, out byte[] result);
        Assert.AreEqual(_expectedContents, result);
    }

    [Test]
    public void CheckCorrectFile()
    {
        byte[] result = Md5Context.File(_location);
        Assert.AreEqual(_expectedFile, result);
    }

    [Test]
    public void CheckFilterId()
    {
        IFilter filter = new XZ();
        Assert.AreEqual(true, filter.Identify(_location));
    }

    [Test]
    public void Test()
    {
        IFilter filter = new XZ();
        Assert.AreEqual(ErrorNumber.NoError, filter.Open(_location));
        Assert.AreEqual(1048576, filter.DataForkLength);
        Assert.AreNotEqual(null, filter.GetDataForkStream());
        Assert.AreEqual(0, filter.ResourceForkLength);
        Assert.AreEqual(null, filter.GetResourceForkStream());
        Assert.AreEqual(false, filter.HasResourceFork);
        filter.Close();
    }
}