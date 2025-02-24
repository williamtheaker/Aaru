// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : BZip2.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Compression algorithms.
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

namespace Aaru.Compression;

using System.IO;
using System.Runtime.InteropServices;
using Ionic.BZip2;

/// <summary>
/// Implements the BZIP2 compression algorithm
/// </summary>
public class BZip2
{
    /// <summary>Set to <c>true</c> if this algorithm is supported, <c>false</c> otherwise.</summary>
    public static bool IsSupported => true;

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_bzip2_decode_buffer(byte[] dstBuffer, ref uint dstSize, byte[] srcBuffer, uint srcSize);

    [DllImport("libAaru.Compression.Native", SetLastError = true)]
    static extern int AARU_bzip2_encode_buffer(byte[] dstBuffer, ref uint dstSize, byte[] srcBuffer, uint srcSize,
                                               int blockSize100K);

    /// <summary>Decodes a buffer compressed with BZIP2</summary>
    /// <param name="source">Encoded buffer</param>
    /// <param name="destination">Buffer where to write the decoded data</param>
    /// <returns>The number of decoded bytes</returns>
    public static int DecodeBuffer(byte[] source, byte[] destination)
    {
        var destinationSize = (uint)destination.Length;

        if(Native.IsSupported)
        {
            AARU_bzip2_decode_buffer(destination, ref destinationSize, source, (uint)source.Length);

            return (int)destinationSize;
        }

        using var cmpMs     = new MemoryStream(source);
        using var decStream = new BZip2InputStream(cmpMs, false);

        return decStream.Read(destination, 0, destination.Length);
    }

    /// <summary>Compresses a buffer using BZIP2</summary>
    /// <param name="source">Data to compress</param>
    /// <param name="destination">Buffer to store the compressed data</param>
    /// <param name="blockSize100K">Block size in 100KiB units</param>
    /// <returns></returns>
    public static int EncodeBuffer(byte[] source, byte[] destination, int blockSize100K)
    {
        var destinationSize = (uint)destination.Length;

        if(Native.IsSupported)
        {
            AARU_bzip2_encode_buffer(destination, ref destinationSize, source, (uint)source.Length, blockSize100K);

            return (int)destinationSize;
        }

        using var cmpMs     = new MemoryStream(source);
        using var encStream = new BZip2OutputStream(new MemoryStream(destination), blockSize100K);
        encStream.Write(source, 0, source.Length);

        return source.Length;
    }
}