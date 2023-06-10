using System;

namespace CannedBytes.Midi.Device;

/// <summary>
/// A flag for each bit in a ushort.
/// </summary>
[Flags]
public enum BitFlags
{
    // single bits
    None = 0x0000,
    Bit0 = 0x0001,
    Bit1 = 0x0002,
    Bit2 = 0x0004,
    Bit3 = 0x0008,
    Bit4 = 0x0010,
    Bit5 = 0x0020,
    Bit6 = 0x0040,
    Bit7 = 0x0080,
    Bit8 = 0x0100,
    Bit9 = 0x0200,
    Bit10 = 0x0400,
    Bit11 = 0x0800,
    Bit12 = 0x1000,
    Bit13 = 0x2000,
    Bit14 = 0x4000,
    Bit15 = 0x8000,

    // multiple bits
    LoByte = 0x00FF,
    HiByte = 0xFF00,
    Word = 0xFFFF,
    DataByte = 0x007F,
    DataWord = 0x7F7F,
    LoByteLoNibble = 0x000F,
    LoByteHiNibble = 0x00F0,
    HiByteLoNibble = 0x0F00,
    HiByteHiNibble = 0xF000,
}
