// /***************************************************************************
// Aaru Data Preservation Suite
// ----------------------------------------------------------------------------
//
// Filename       : Structs.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : CP/M filesystem plugin.
//
// --[ Description ] ----------------------------------------------------------
//
//     CP/M filesystem structures.
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



// ReSharper disable NotAccessedField.Local

namespace Aaru.Filesystems;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

public sealed partial class CPM
{
    /// <summary>Most of the times this structure is hard wired or generated by CP/M, not stored on disk</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class DiscParameterBlock
    {
        /// <summary>First byte of allocation bitmap</summary>
        public byte al0;
        /// <summary>Second byte of allocation bitmap</summary>
        public byte al1;
        /// <summary>Block mask</summary>
        public byte blm;
        /// <summary>Block shift</summary>
        public byte bsh;
        /// <summary>Checksum vector size</summary>
        public ushort cks;
        /// <summary>Directory entries - 1</summary>
        public ushort drm;
        /// <summary>Blocks on disk - 1</summary>
        public ushort dsm;
        /// <summary>Extent mask</summary>
        public byte exm;
        /// <summary>Reserved tracks</summary>
        public ushort off;
        /// <summary>Physical sector mask</summary>
        public byte phm;
        /// <summary>Physical sector shift</summary>
        public byte psh;
        /// <summary>Sectors per track</summary>
        public ushort spt;
    }

    /// <summary>Amstrad superblock, for PCW</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct AmstradSuperBlock
    {
        /// <summary>Format ID. 0 single-side, 3 double-side. 1 and 2 are for CPC but they don't use the superblock</summary>
        public readonly byte format;
        /// <summary>Gives information about side ordering</summary>
        public readonly byte sidedness;
        /// <summary>Tracks per side, aka, cylinders</summary>
        public readonly byte tps;
        /// <summary>Sectors per track</summary>
        public readonly byte spt;
        /// <summary>Physical sector shift</summary>
        public readonly byte psh;
        /// <summary>Reserved tracks</summary>
        public readonly byte off;
        /// <summary>Block size shift</summary>
        public readonly byte bsh;
        /// <summary>How many blocks does the directory take</summary>
        public readonly byte dirBlocks;
        /// <summary>GAP#3 length (intersector)</summary>
        public readonly byte gapLen;
        /// <summary>GAP#4 length (end-of-track)</summary>
        public readonly byte formatGap;
        /// <summary>Must be 0</summary>
        public readonly byte zero1;
        /// <summary>Must be 0</summary>
        public readonly byte zero2;
        /// <summary>Must be 0</summary>
        public readonly byte zero3;
        /// <summary>Must be 0</summary>
        public readonly byte zero4;
        /// <summary>Must be 0</summary>
        public readonly byte zero5;
        /// <summary>Indicates machine the boot code following the superblock is designed to boot</summary>
        public readonly byte fiddle;
    }

    /// <summary>Superblock found on CP/M-86 hard disk volumes</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct HardDiskSuperBlock
    {
        /// <summary>Value so the sum of all the superblock's sector bytes taken as 16-bit values gives 0</summary>
        public readonly ushort checksum;
        /// <summary>Copyright string</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1F)]
        public readonly byte[] copyright;
        /// <summary>First cylinder of disk where this volume resides</summary>
        public readonly ushort firstCylinder;
        /// <summary>How many cylinders does this volume span</summary>
        public readonly ushort cylinders;
        /// <summary>Heads on hard disk</summary>
        public readonly byte heads;
        /// <summary>Sectors per track</summary>
        public readonly byte sectorsPerTrack;
        /// <summary>Flags, only use by CCP/M where bit 0 equals verify on write</summary>
        public readonly byte flags;
        /// <summary>Sector size / 128</summary>
        public readonly byte recordsPerSector;
        /// <summary>
        ///     <see cref="DiscParameterBlock.spt" />
        /// </summary>
        public readonly ushort spt;
        /// <summary>
        ///     <see cref="DiscParameterBlock.bsh" />
        /// </summary>
        public readonly byte bsh;
        /// <summary>
        ///     <see cref="DiscParameterBlock.blm" />
        /// </summary>
        public readonly byte blm;
        /// <summary>
        ///     <see cref="DiscParameterBlock.exm" />
        /// </summary>
        public readonly byte exm;
        /// <summary>
        ///     <see cref="DiscParameterBlock.dsm" />
        /// </summary>
        public readonly ushort dsm;
        /// <summary>
        ///     <see cref="DiscParameterBlock.drm" />
        /// </summary>
        public readonly ushort drm;
        /// <summary>
        ///     <see cref="DiscParameterBlock.al0" />
        /// </summary>
        public readonly ushort al0;
        /// <summary>
        ///     <see cref="DiscParameterBlock.al1" />
        /// </summary>
        public readonly ushort al1;
        /// <summary>
        ///     <see cref="DiscParameterBlock.cks" />
        /// </summary>
        public readonly ushort cks;
        /// <summary>
        ///     <see cref="DiscParameterBlock.off" />
        /// </summary>
        public readonly ushort off;
        /// <summary>Must be zero</summary>
        public readonly ushort zero1;
        /// <summary>Must be zero</summary>
        public readonly ushort zero2;
        /// <summary>Must be zero</summary>
        public readonly ushort zero3;
        /// <summary>Must be zero</summary>
        public readonly ushort zero4;
        /// <summary>How many 128 bytes are in a block</summary>
        public readonly ushort recordsPerBlock;
        /// <summary>Maximum number of bad blocks in the bad block list</summary>
        public readonly ushort badBlockWordsMax;
        /// <summary>Used number of bad blocks in the bad block list</summary>
        public readonly ushort badBlockWords;
        /// <summary>First block after the blocks reserved for bad block substitution</summary>
        public readonly ushort firstSub;
    }

    /// <summary>Volume label entry</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct LabelEntry
    {
        /// <summary>Must be 0x20</summary>
        public readonly byte signature;
        /// <summary>Label in ASCII</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public readonly byte[] label;
        /// <summary>
        ///     Label flags. Bit 0 = label exists, bit 4 = creation timestamp, bit 5 = modification timestamp, bit 6 = access
        ///     timestamp, bit 7 = password enabled
        /// </summary>
        public readonly byte flags;
        /// <summary>Password decoder byte</summary>
        public readonly byte passwordDecoder;
        /// <summary>Must be 0</summary>
        public readonly ushort reserved;
        /// <summary>Password XOR'ed with <see cref="passwordDecoder" /></summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] password;
        /// <summary>Label creation time</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] ctime;
        /// <summary>Label modification time</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] mtime;
    }

    /// <summary>CP/M 3 timestamp entry</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct DateEntry
    {
        /// <summary>Must be 0x21</summary>
        public readonly byte signature;
        /// <summary>File 1 create/access timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date1;
        /// <summary>File 1 modification timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date2;
        /// <summary>File 1 password mode</summary>
        public readonly byte mode1;
        public readonly byte zero1;
        /// <summary>File 2 create/access timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date3;
        /// <summary>File 2 modification timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date4;
        /// <summary>File 2 password mode</summary>
        public readonly byte mode2;
        public readonly byte zero2;
        /// <summary>File 3 create/access timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date5;
        /// <summary>File 3 modification timestamp</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] date6;
        /// <summary>File 3 password mode</summary>
        public readonly byte mode3;
        public readonly ushort zero3;
    }

    /// <summary>Password entry</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct PasswordEntry
    {
        /// <summary>16 + user number</summary>
        public readonly byte userNumber;
        /// <summary>Filename</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] filename;
        /// <summary>Extension</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly byte[] extension;
        /// <summary>Password mode. Bit 7 = required for read, bit 6 = required for write, bit 5 = required for delete</summary>
        public readonly byte mode;
        /// <summary>Password decoder byte</summary>
        public readonly byte passwordDecoder;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly byte[] reserved;
        /// <summary>Password XOR'ed with <see cref="passwordDecoder" /></summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] password;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] reserved2;
    }

    /// <summary>Timestamp for Z80DOS or DOS+</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct TrdPartyDateEntry
    {
        /// <summary>Must be 0x21</summary>
        public readonly byte signature;
        public readonly byte zero;
        /// <summary>Creation year for file 1</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly byte[] create1;
        /// <summary>Modification time for file 1</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] modify1;
        /// <summary>Access time for file 1</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] access1;
        /// <summary>Creation year for file 2</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly byte[] create2;
        /// <summary>Modification time for file 2</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] modify2;
        /// <summary>Access time for file 2</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] access2;
        /// <summary>Creation year for file 3</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public readonly byte[] create3;
        /// <summary>Modification time for file 3</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] modify3;
        /// <summary>Access time for file 3</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly byte[] access3;
    }

    /// <summary>Directory entry for &lt;256 allocation blocks</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct DirectoryEntry
    {
        /// <summary>User number. Bit 7 set in CP/M 1 means hidden</summary>
        public readonly byte statusUser;
        /// <summary>Filename and bit 7 as flags</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] filename;
        /// <summary>Extension and bit 7 as flags</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly byte[] extension;
        /// <summary>Low byte of extent number</summary>
        public readonly byte extentCounter;
        /// <summary>
        ///     Last record bytes. In some implementations it means how many bytes are used in the last record, in others how
        ///     many bytes are free. It always refer to 128 byte records even if blocks are way bigger, so it's mostly useless.
        /// </summary>
        public readonly byte lastRecordBytes;
        /// <summary>High byte of extent number</summary>
        public readonly byte extentCounterHigh;
        /// <summary>How many records are used in this entry. 0x80 if all are used.</summary>
        public readonly byte records;
        /// <summary>Allocation blocks</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public readonly byte[] allocations;
    }

    /// <summary>Directory entry for &gt;256 allocation blocks</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct DirectoryEntry16
    {
        /// <summary>User number. Bit 7 set in CP/M 1 means hidden</summary>
        public readonly byte statusUser;
        /// <summary>Filename and bit 7 as flags</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly byte[] filename;
        /// <summary>Extension and bit 7 as flags</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly byte[] extension;
        /// <summary>Low byte of extent number</summary>
        public readonly byte extentCounter;
        /// <summary>
        ///     Last record bytes. In some implementations it means how many bytes are used in the last record, in others how
        ///     many bytes are free. It always refer to 128 byte records even if blocks are way bigger, so it's mostly useless.
        /// </summary>
        public readonly byte lastRecordBytes;
        /// <summary>High byte of extent number</summary>
        public readonly byte extentCounterHigh;
        /// <summary>How many records are used in this entry. 0x80 if all are used.</summary>
        public readonly byte records;
        /// <summary>Allocation blocks</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public readonly ushort[] allocations;
    }
}