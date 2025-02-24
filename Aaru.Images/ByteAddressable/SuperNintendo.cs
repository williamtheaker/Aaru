namespace Aaru.DiscImages.ByteAddressable;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Aaru.CommonTypes;
using Aaru.CommonTypes.Enums;
using Aaru.CommonTypes.Interfaces;
using Aaru.CommonTypes.Structs;
using Aaru.Helpers;
using Schemas;
using Marshal = Aaru.Helpers.Marshal;

public class SuperNintendo : IByteAddressableImage
{
    byte[]    _data;
    Stream    _dataStream;
    Header    _header;
    ImageInfo _imageInfo;
    bool      _opened;
    /// <inheritdoc />
    public ImageInfo Info => _imageInfo;
    /// <inheritdoc />
    public string Name => "Super Nintendo";

    /// <inheritdoc />
    public bool Identify(IFilter imageFilter)
    {
        if(imageFilter == null)
            return false;

        Stream stream = imageFilter.GetDataForkStream();

        // Not sure but seems to be a multiple of at least this
        if(stream.Length % 32768 != 0)
            return false;

        // Too many false positives at bigger sizes
        if(stream.Length > 16 * 1048576)
            return false;

        Header header;
        var    headerBytes = new byte[48];

        if(stream.Length > 0x40FFFF)
        {
            stream.Position = 0x40FFB0;

            stream.Read(headerBytes, 0, 48);
            header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

            if((header.Mode & 0xF) == 0x5 ||
               (header.Mode & 0xF) == 0xA)
                return true;
        }

        if(stream.Length > 0xFFFF)
        {
            stream.Position = 0xFFB0;

            stream.Read(headerBytes, 0, 48);
            header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

            if((header.Mode & 0xF) == 0x1 ||
               (header.Mode & 0xF) == 0xA)
                return true;
        }

        if(stream.Length <= 0x7FFF)
            return false;

        stream.Position = 0x7FB0;

        stream.Read(headerBytes, 0, 48);
        header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

        return (header.Mode & 0xF) == 0x0 || (header.Mode & 0xF) == 0x2 || (header.Mode & 0xF) == 0x3;
    }

    /// <inheritdoc />
    public ErrorNumber Open(IFilter imageFilter)
    {
        if(imageFilter == null)
            return ErrorNumber.NoSuchFile;

        Stream stream = imageFilter.GetDataForkStream();

        // Not sure but seems to be a multiple of at least this
        if(stream.Length % 32768 != 0)
            return ErrorNumber.InvalidArgument;

        var found       = false;
        var headerBytes = new byte[48];

        if(stream.Length > 0x40FFFF)
        {
            stream.Position = 0x40FFB0;

            stream.Read(headerBytes, 0, 48);
            _header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

            if((_header.Mode & 0xF) == 0x5 ||
               (_header.Mode & 0xF) == 0xA)
                found = true;
        }

        if(stream.Length > 0xFFFF)
        {
            stream.Position = 0xFFB0;

            stream.Read(headerBytes, 0, 48);
            _header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

            if((_header.Mode & 0xF) == 0x1 ||
               (_header.Mode & 0xF) == 0xA)
                found = true;
        }

        if(stream.Length > 0x7FFF)
        {
            stream.Position = 0x7FB0;

            stream.Read(headerBytes, 0, 48);
            _header = Marshal.ByteArrayToStructureLittleEndian<Header>(headerBytes);

            if((_header.Mode & 0xF) == 0x0 ||
               (_header.Mode & 0xF) == 0x2 ||
               (_header.Mode & 0xF) == 0x3)
                found = true;
        }

        if(!found)
            return ErrorNumber.InvalidArgument;

        _data           = new byte[imageFilter.DataForkLength];
        stream.Position = 0;
        stream.Read(_data, 0, (int)imageFilter.DataForkLength);

        Encoding encoding;

        try
        {
            encoding = Encoding.GetEncoding("shift_jis");
        }
        catch(Exception)
        {
            encoding = Encoding.ASCII;
        }

        _imageInfo = new ImageInfo
        {
            Application          = "Multi Game Doctor 2",
            CreationTime         = imageFilter.CreationTime,
            ImageSize            = (ulong)imageFilter.DataForkLength,
            MediaType            = _header.Region == 1 ? MediaType.SNESGamePakUS : MediaType.SNESGamePak,
            LastModificationTime = imageFilter.LastWriteTime,
            Sectors              = (ulong)imageFilter.DataForkLength,
            XmlMediaType         = XmlMediaType.LinearMedia,
            MediaTitle           = StringHandlers.SpacePaddedToString(_header.Title, encoding),
            MediaManufacturer    = DecodeManufacturer(_header.OldMakerCode, _header.MakerCode)
        };

        var sb = new StringBuilder();

        sb.AppendFormat("Name: {0}", _imageInfo.MediaTitle).AppendLine();
        sb.AppendFormat("Manufacturer: {0}", _imageInfo.MediaManufacturer).AppendLine();
        sb.AppendFormat("Region: {0}", DecodeRegion(_header.Region)).AppendLine();

        if(_header.OldMakerCode == 0x33)
            sb.AppendFormat("Game code: {0}", _header.GameCode).AppendLine();

        sb.AppendFormat("Revision: {0}", _header.Revision).AppendLine();

        if(_header.OldMakerCode == 0x33)
            sb.AppendFormat("Special revision: {0}", _header.SpecialVersion).AppendLine();

        sb.AppendFormat("Header checksum: 0x{0:X4}", _header.Checksum).AppendLine();
        sb.AppendFormat("Header checksum complement: 0x{0:X4}", _header.ChecksumComplement).AppendLine();

        sb.AppendFormat("ROM size: {0} bytes", (1 << _header.RomSize) * 1024).AppendLine();

        if(_header.RamSize > 0)
            sb.AppendFormat("RAM size: {0} bytes", (1 << _header.RamSize) * 1024).AppendLine();

        if(_header.OldMakerCode == 0x33)
        {
            if(_header.ExpansionFlashSize > 0)
                sb.AppendFormat("Flash size: {0} bytes", (1 << _header.ExpansionFlashSize) * 1024).AppendLine();

            if(_header.ExpansionRamSize > 0)
                sb.AppendFormat("Expansion RAM size: {0} bytes", (1 << _header.ExpansionRamSize) * 1024).AppendLine();
        }

        sb.AppendFormat("Cartridge type: {0}", DecodeCartType(_header.Mode)).AppendLine();
        sb.AppendFormat("ROM speed: {0}", DecodeRomSpeed(_header.Mode)).AppendLine();
        sb.AppendFormat("Bank size: {0} bytes", DecodeBankSize(_header.Mode)).AppendLine();
        sb.AppendFormat("Cartridge chip set: {0}", DecodeChipset(_header.Chipset)).AppendLine();
        sb.AppendFormat("Coprocessor: {0}", DecodeCoprocessor(_header.Chipset, _header.Subtype)).AppendLine();

        _imageInfo.Comments = sb.ToString();
        _opened             = true;

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public Guid Id => new("DF861EB0-8B9B-4E3F-BF39-9F2E75668F80");
    /// <inheritdoc />
    public string Author => "Natalia Portillo";
    /// <inheritdoc />
    public string Format => "Super Nintendo Cartridge Dump";
    /// <inheritdoc />
    public List<DumpHardwareType> DumpHardware => null;
    /// <inheritdoc />
    public CICMMetadataType CicmMetadata => null;
    /// <inheritdoc />
    public string ErrorMessage { get; private set; }
    /// <inheritdoc />
    public bool IsWriting { get; private set; }
    /// <inheritdoc />
    public IEnumerable<string> KnownExtensions => new[]
    {
        ".sfc"
    };
    /// <inheritdoc />
    public IEnumerable<MediaTagType> SupportedMediaTags => Array.Empty<MediaTagType>();
    /// <inheritdoc />
    public IEnumerable<MediaType> SupportedMediaTypes => new[]
    {
        MediaType.SNESGamePak, MediaType.SNESGamePakUS
    };
    /// <inheritdoc />
    public IEnumerable<(string name, Type type, string description, object @default)> SupportedOptions =>
        Array.Empty<(string name, Type type, string description, object @default)>();
    /// <inheritdoc />
    public IEnumerable<SectorTagType> SupportedSectorTags => Array.Empty<SectorTagType>();

    /// <inheritdoc />
    public bool Create(string path, MediaType mediaType, Dictionary<string, string> options, ulong sectors,
                       uint sectorSize) => Create(path, mediaType, options, (long)sectors) == ErrorNumber.NoError;

    /// <inheritdoc />
    public bool Close()
    {
        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return false;
        }

        if(!IsWriting)
        {
            ErrorMessage = "Image is not opened for writing.";

            return false;
        }

        _dataStream.Position = 0;
        _dataStream.Write(_data, 0, _data.Length);
        _dataStream.Close();

        IsWriting = false;
        _opened   = false;

        return true;
    }

    /// <inheritdoc />
    public bool SetCicmMetadata(CICMMetadataType metadata) => false;

    /// <inheritdoc />
    public bool SetDumpHardware(List<DumpHardwareType> dumpHardware) => false;

    /// <inheritdoc />
    public bool SetMetadata(ImageInfo metadata) => true;

    /// <inheritdoc />
    public long Position { get; set; }

    /// <inheritdoc />
    public ErrorNumber Create(string path, MediaType mediaType, Dictionary<string, string> options, long maximumSize)
    {
        if(_opened)
        {
            ErrorMessage = "Cannot create an opened image";

            return ErrorNumber.InvalidArgument;
        }

        if(mediaType != MediaType.SNESGamePak &&
           mediaType != MediaType.SNESGamePakUS)
        {
            ErrorMessage = $"Unsupported media format {mediaType}";

            return ErrorNumber.NotSupported;
        }

        _imageInfo = new ImageInfo
        {
            MediaType = mediaType,
            Sectors   = (ulong)maximumSize
        };

        try
        {
            _dataStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        catch(IOException e)
        {
            ErrorMessage = $"Could not create new image file, exception {e.Message}";

            return ErrorNumber.InOutError;
        }

        _imageInfo.MediaType = mediaType;
        IsWriting            = true;
        _opened              = true;
        _data                = new byte[maximumSize];

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public ErrorNumber GetMappings(out LinearMemoryMap mappings)
    {
        mappings = new LinearMemoryMap();

        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        int chipset = _header.Chipset & 0xF;

        bool hasRam      = _header.RamSize > 0;
        bool hasExtraRam = _header.OldMakerCode == 0x33 && _header.ExpansionRamSize   > 0;
        bool hasFlash    = _header.OldMakerCode == 0x33 && _header.ExpansionFlashSize > 0;
        bool hasBattery  = chipset is 2 or 5 or 6 or 9 or 0xA;

        var devices = 1;

        if(hasRam)
            devices++;

        if(hasExtraRam)
            devices++;

        if(hasFlash)
            devices++;

        mappings         = new LinearMemoryMap();
        mappings.Devices = new LinearMemoryDevice[devices];

        mappings.Devices[0] = new LinearMemoryDevice
        {
            Type = LinearMemoryType.ROM,
            PhysicalAddress = new LinearMemoryAddressing
            {
                Start  = 0,
                Length = (ulong)_data.Length
            }
        };

        var pos  = 1;
        var addr = (ulong)_data.Length;

        if(hasRam)
        {
            mappings.Devices[pos] = new LinearMemoryDevice
            {
                Type = hasBattery ? LinearMemoryType.SaveRAM : LinearMemoryType.WorkRAM,
                PhysicalAddress = new LinearMemoryAddressing
                {
                    Start  = addr,
                    Length = (ulong)(1 << _header.RamSize) * 1024
                }
            };

            addr += (ulong)(1 << _header.RamSize) * 1024;
            pos++;
        }

        if(hasExtraRam)
        {
            mappings.Devices[pos] = new LinearMemoryDevice
            {
                Type = hasBattery && !hasRam ? LinearMemoryType.SaveRAM : LinearMemoryType.WorkRAM,
                PhysicalAddress = new LinearMemoryAddressing
                {
                    Start  = addr,
                    Length = (ulong)(1 << _header.ExpansionRamSize) * 1024
                }
            };

            addr += (ulong)(1 << _header.ExpansionRamSize) * 1024;
            pos++;
        }

        if(hasFlash)
            mappings.Devices[pos] = new LinearMemoryDevice
            {
                Type = LinearMemoryType.NOR,
                PhysicalAddress = new LinearMemoryAddressing
                {
                    Start  = addr,
                    Length = (ulong)(1 << _header.ExpansionRamSize) * 1024
                }
            };

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public ErrorNumber ReadByte(out byte b, bool advance = true) => ReadByteAt(Position, out b, advance);

    /// <inheritdoc />
    public ErrorNumber ReadByteAt(long position, out byte b, bool advance = true)
    {
        b = 0;

        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        if(position >= _data.Length)
        {
            ErrorMessage = "The requested position is out of range.";

            return ErrorNumber.OutOfRange;
        }

        b = _data[position];

        if(advance)
            Position = position + 1;

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public ErrorNumber ReadBytes(byte[] buffer, int offset, int bytesToRead, out int bytesRead, bool advance = true) =>
        ReadBytesAt(Position, buffer, offset, bytesToRead, out bytesRead, advance);

    /// <inheritdoc />
    public ErrorNumber ReadBytesAt(long position, byte[] buffer, int offset, int bytesToRead, out int bytesRead,
                                   bool advance = true)
    {
        bytesRead = 0;

        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        if(position >= _data.Length)
        {
            ErrorMessage = "The requested position is out of range.";

            return ErrorNumber.OutOfRange;
        }

        if(buffer is null)
        {
            ErrorMessage = "Buffer must not be null.";

            return ErrorNumber.InvalidArgument;
        }

        if(offset + bytesToRead > buffer.Length)
            bytesRead = buffer.Length - offset;

        if(position + bytesToRead > _data.Length)
            bytesToRead = (int)(_data.Length - position);

        Array.Copy(_data, position, buffer, offset, bytesToRead);

        if(advance)
            Position = position + bytesToRead;

        bytesRead = bytesToRead;

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public ErrorNumber SetMappings(LinearMemoryMap mappings)
    {
        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        if(!IsWriting)
        {
            ErrorMessage = "Image is not opened for writing.";

            return ErrorNumber.ReadOnly;
        }

        var foundRom      = false;
        var foundRam      = false;
        var foundExtraRam = false;
        var foundFlash    = false;

        // Sanitize
        foreach(LinearMemoryDevice map in mappings.Devices)
            switch(map.Type)
            {
                case LinearMemoryType.ROM when !foundRom:
                    foundRom = true;

                    break;
                case LinearMemoryType.SaveRAM when !foundRam:
                    foundRam = true;

                    break;
                case LinearMemoryType.SaveRAM when !foundExtraRam:
                    foundExtraRam = true;

                    break;
                case LinearMemoryType.WorkRAM when !foundRam:
                    foundRam = true;

                    break;
                case LinearMemoryType.WorkRAM when !foundExtraRam:
                    foundExtraRam = true;

                    break;
                case LinearMemoryType.NOR when !foundFlash:
                case LinearMemoryType.NAND when !foundFlash:
                    foundFlash = true;

                    break;
                default: return ErrorNumber.InvalidArgument;
            }

        // Cannot save in this image format anyway
        return foundRom ? ErrorNumber.NoError : ErrorNumber.InvalidArgument;
    }

    /// <inheritdoc />
    public ErrorNumber WriteByte(byte b, bool advance = true) => WriteByteAt(Position, b, advance);

    /// <inheritdoc />
    public ErrorNumber WriteByteAt(long position, byte b, bool advance = true)
    {
        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        if(!IsWriting)
        {
            ErrorMessage = "Image is not opened for writing.";

            return ErrorNumber.ReadOnly;
        }

        if(position >= _data.Length)
        {
            ErrorMessage = "The requested position is out of range.";

            return ErrorNumber.OutOfRange;
        }

        _data[position] = b;

        if(advance)
            Position = position + 1;

        return ErrorNumber.NoError;
    }

    /// <inheritdoc />
    public ErrorNumber WriteBytes(byte[] buffer, int offset, int bytesToWrite, out int bytesWritten,
                                  bool advance = true) =>
        WriteBytesAt(Position, buffer, offset, bytesToWrite, out bytesWritten, advance);

    /// <inheritdoc />
    public ErrorNumber WriteBytesAt(long position, byte[] buffer, int offset, int bytesToWrite, out int bytesWritten,
                                    bool advance = true)
    {
        bytesWritten = 0;

        if(!_opened)
        {
            ErrorMessage = "Not image has been opened.";

            return ErrorNumber.NotOpened;
        }

        if(!IsWriting)
        {
            ErrorMessage = "Image is not opened for writing.";

            return ErrorNumber.ReadOnly;
        }

        if(position >= _data.Length)
        {
            ErrorMessage = "The requested position is out of range.";

            return ErrorNumber.OutOfRange;
        }

        if(buffer is null)
        {
            ErrorMessage = "Buffer must not be null.";

            return ErrorNumber.InvalidArgument;
        }

        if(offset + bytesToWrite > buffer.Length)
            bytesToWrite = buffer.Length - offset;

        if(position + bytesToWrite > _data.Length)
            bytesToWrite = (int)(_data.Length - position);

        Array.Copy(buffer, offset, _data, position, bytesToWrite);

        if(advance)
            Position = position + bytesToWrite;

        bytesWritten = bytesToWrite;

        return ErrorNumber.NoError;
    }

    static string DecodeCoprocessor(byte chipset, byte subtype)
    {
        if((chipset & 0xF) < 3)
            return "None";

        return ((chipset & 0xF0) >> 4) switch
               {
                   0   => "DSP",
                   1   => "GSU",
                   2   => "OBC1",
                   3   => "SA-1",
                   4   => "S-DD1",
                   5   => "S-RTC",
                   0xE => "Other",
                   0xF => subtype switch
                          {
                              0    => "SPC7110",
                              1    => "ST010/ST011",
                              2    => "ST018",
                              0x10 => "CX4",
                              _    => "Unknown"
                          },
                   _ => "Unknown"
               };
    }

    static string DecodeChipset(byte chipset)
    {
        switch(chipset & 0xF)
        {
            case 0:                            return "ROM";
            case 1:                            return "ROM and RAM";
            case 2 when (chipset & 0xF0) == 0: return "ROM, RAM and battery";
            case 3:                            return "ROM and coprocessor";
            case 4:                            return "ROM, RAM and coprocessor";
            case 2:
            case 5: return "ROM, RAM, battery and coprocessor";
            case 6:   return "ROM, battery and coprocessor";
            case 9:   return "ROM, RAM, battery, coprocessor and RTC";
            case 0xA: return "ROM, RAM, battery and coprocessor";
            default:  return "Unknown";
        }
    }

    static int DecodeBankSize(byte mode)
    {
        switch(mode & 0xF)
        {
            case 0:
            case 2:
            case 3: return 32768;
            case 1:
            case 5:
            case 0xA: return 65536;
            default: return 0;
        }
    }

    static string DecodeRomSpeed(byte mode) => (mode & 0x10) == 0x10 ? "Fast (120ns)" : "Slow (200ns)";

    static string DecodeCartType(byte mode)
    {
        switch(mode & 0xF)
        {
            case 0:
            case 2:
            case 3: return "LoROM";
            case 1:
            case 0xA: return "HiROM";
            case 5:  return "ExHiROM";
            default: return "Unknown";
        }
    }

    static string DecodeRegion(byte headerRegion) => headerRegion switch
                                                     {
                                                         0  => "Japan",
                                                         1  => "USA and Canada",
                                                         2  => "Europe, Oceania, Asia",
                                                         3  => "Sweden/Scandinavia",
                                                         4  => "Finland",
                                                         5  => "Denmark",
                                                         6  => "France",
                                                         7  => "Netherlands",
                                                         8  => "Spain",
                                                         9  => "Germany, Austria, Switzerland",
                                                         10 => "Italy",
                                                         11 => "China, Hong Kong",
                                                         12 => "Indonesia",
                                                         13 => "South Korea",
                                                         15 => "Canada",
                                                         16 => "Brazil",
                                                         17 => "Australia",
                                                         _  => "Unknown"
                                                     };

    static string DecodeManufacturer(byte oldMakerCode, string makerCode)
    {
        // TODO: Add full table
        if(oldMakerCode != 0x33)
            makerCode = $"{(oldMakerCode >> 4) * 36 + (oldMakerCode & 0x0f)}";

        return makerCode switch
               {
                   "01" => "Nintendo",
                   _    => "Unknown"
               };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1), SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local"),
     SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    struct Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string MakerCode;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string GameCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Reserved;
        public byte ExpansionFlashSize;
        public byte ExpansionRamSize;
        public byte SpecialVersion;
        public byte Subtype;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public byte[] Title;
        public byte   Mode;
        public byte   Chipset;
        public byte   RomSize;
        public byte   RamSize;
        public byte   Region;
        public byte   OldMakerCode;
        public byte   Revision;
        public ushort ChecksumComplement;
        public ushort Checksum;
    }
}