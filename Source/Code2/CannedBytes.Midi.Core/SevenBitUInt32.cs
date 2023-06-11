using System;

namespace CannedBytes.Midi.Core;

/// <summary>
/// Represents a seven-bit value for a maximum of 4 (7 bit) bytes.
/// </summary>
public struct SevenBitUInt32 : IConvertible
{
    /// <summary>
    /// A normal .net native value with top 4 bits always clear.
    /// </summary>
    private uint _value;

    /// <summary>
    /// A conventional (internal) value.
    /// </summary>
    /// <param name="internalValue"></param>
    private SevenBitUInt32(uint internalValue)
    {
        ValidateInternalValue(internalValue);

        _value = internalValue;
    }

    /// <summary>
    /// A parsing ctor.
    /// </summary>
    /// <param name="sevenBitValueString">A string that represents a valid (7 bit) value.</param>
    public SevenBitUInt32(string sevenBitValueString)
    {
        _value = 0;

        if (!ParseInternal(sevenBitValueString))
        {
            throw new FormatException(
                "The string could not be parsed into a SeventBitUInt32: " + sevenBitValueString);
        }
    }

    /// <summary>
    /// Makes any conventional value a seven bit uint. Will throw exceptions if value is too large.
    /// </summary>
    /// <param name="value">Negative values will wrap to uint.</param>
    /// <returns>Returns a new instance.</returns>
    public static SevenBitUInt32 FromInt32(int value)
    {
        return new SevenBitUInt32((uint)value);
    }

    /// <summary>
    /// Constructs from a 7-bit value.
    /// </summary>
    /// <param name="value">Value must match mask 0x7F7F7F7F.</param>
    /// <returns>Returns a new instance.</returns>
    public static SevenBitUInt32 FromSevenBitValue(uint value)
    {
        return new SevenBitUInt32(ToInternal(value));
    }

    /// <summary>
    /// Parses a 7-bit string into a new instance.
    /// </summary>
    /// <param name="s">Must not be null or empty.</param>
    /// <returns>Returns a new instance.</returns>
    public static SevenBitUInt32 Parse(string s)
    {
        return new SevenBitUInt32(s);
    }

    /// <summary>
    /// Attempts to parse the <paramref name="s"/>tring into a new instance.
    /// </summary>
    /// <param name="s">Must not be null or empty.</param>
    /// <param name="value">Receives the new instance if successful.</param>
    /// <returns>Returns true if successful.</returns>
    public static bool TryParse(string s, out SevenBitUInt32 value)
    {
        value = new SevenBitUInt32();

        return value.ParseInternal(s);
    }

    private bool ParseInternal(string s)
    {
        bool success = ValueParser.TryParseToBytes(s, Ordering.LittleEndian, out byte[] bytes);

        if (success)
        {
            Bytes = bytes;
        }

        return success;
    }

    /// <summary>
    /// A zero instance.
    /// </summary>
    public static readonly SevenBitUInt32 Zero = new((uint)0);

    /// <summary>
    /// Converts to an int (may turn out negative).
    /// </summary>
    /// <returns>Returns the value cast to int.</returns>
    public int ToInt32()
    {
        return (int)ToUInt32();
    }

    /// <summary>
    /// Returns the 7-bit value.
    /// </summary>
    /// <returns>Value matches mask: 0x7F7F7F7F.</returns>
    public uint ToUInt32()
    {
        return ToSevenBit(_value);
    }

    /// <summary>
    /// Gets or sets LittleEndian ordered individual bytes (length = 4).
    /// </summary>
    public byte[] Bytes
    {
        get { return ByteConverter.FromUint32ToSevenBitBytes(_value, Ordering.LittleEndian); }
        private set { _value = ByteConverter.FromSevenBitBytesToUInt32(value, Ordering.LittleEndian); }
    }

    public bool Equals(SevenBitUInt32 thatValue)
    {
        return _value == thatValue._value;
    }

    public override bool Equals(object obj)
    {
        if (obj is SevenBitUInt32)
        {
            return Equals((SevenBitUInt32)obj);
        }
        if (obj is uint)
        {
            return _value == (uint)obj;
        }
        if (obj is int)
        {
            return ToInt32() == (int)obj;
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <summary>
    /// Outputs the 7-bite value in hex and dec.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ToString("X") + "H (" + ToString("D") + ")";
    }

    public string ToString(string format)
    {
        uint value = ToUInt32();

        return value.ToString(format);
    }

    #region Operators

    public static SevenBitUInt32 operator +(SevenBitUInt32 left, SevenBitUInt32 right)
    {
        return new SevenBitUInt32(left._value + right._value);
    }

    public static SevenBitUInt32 operator +(SevenBitUInt32 left, int right)
    {
        return new SevenBitUInt32((uint)(left._value + right));
    }

    public static SevenBitUInt32 operator -(SevenBitUInt32 left, SevenBitUInt32 right)
    {
        return new SevenBitUInt32(left._value - right._value);
    }

    public static SevenBitUInt32 operator -(SevenBitUInt32 left, int right)
    {
        return new SevenBitUInt32((uint)(left._value - right));
    }

    public static implicit operator int(SevenBitUInt32 value)
    {
        return value.ToInt32();
    }

    public static bool operator ==(SevenBitUInt32 thisValue, SevenBitUInt32 thatValue)
    {
        return thisValue.Equals(thatValue);
    }

    public static bool operator !=(SevenBitUInt32 thisValue, SevenBitUInt32 thatValue)
    {
        return !thisValue.Equals(thatValue);
    }

    #endregion

    #region IConvertible interface

    TypeCode IConvertible.GetTypeCode()
    {
        return TypeCode.UInt32;
    }

    bool IConvertible.ToBoolean(IFormatProvider provider)
    {
        return _value != 0;
    }

    byte IConvertible.ToByte(IFormatProvider provider)
    {
        return (byte)ToUInt32();
    }

    char IConvertible.ToChar(IFormatProvider provider)
    {
        return (char)ToInt32();
    }

    DateTime IConvertible.ToDateTime(IFormatProvider provider)
    {
        throw new NotSupportedException();
    }

    decimal IConvertible.ToDecimal(IFormatProvider provider)
    {
        return (decimal)ToUInt32();
    }

    double IConvertible.ToDouble(IFormatProvider provider)
    {
        return (double)ToUInt32();
    }

    short IConvertible.ToInt16(IFormatProvider provider)
    {
        return (short)ToInt32();
    }

    int IConvertible.ToInt32(IFormatProvider provider)
    {
        return ToInt32();
    }

    long IConvertible.ToInt64(IFormatProvider provider)
    {
        return ToInt32();
    }

    sbyte IConvertible.ToSByte(IFormatProvider provider)
    {
        return (sbyte)ToInt32();
    }

    float IConvertible.ToSingle(IFormatProvider provider)
    {
        return (float)ToUInt32();
    }

    string IConvertible.ToString(IFormatProvider provider)
    {
        return ToString();
    }

    object IConvertible.ToType(Type conversionType, IFormatProvider provider)
    {
        throw new NotSupportedException();
    }

    ushort IConvertible.ToUInt16(IFormatProvider provider)
    {
        return (ushort)ToUInt32();
    }

    uint IConvertible.ToUInt32(IFormatProvider provider)
    {
        return ToUInt32();
    }

    ulong IConvertible.ToUInt64(IFormatProvider provider)
    {
        return ToUInt32();
    }

    #endregion

    // four bytes of 7-bits leaves 4 top-bits unused.
    private const uint IntMaskAll = 0x0FFFFFFF;
    private const uint IntMaskByte1 = 0x0000007F;
    private const uint IntMaskByte2 = 0x00003F80;
    private const uint IntMaskByte3 = 0x001FC000;
    private const uint IntMaskByte4 = 0x0FE00000;

    private static uint ToSevenBit(uint internalValue)
    {
        ValidateInternalValue(internalValue);

        uint sevenBitValue = (internalValue & IntMaskByte1) << 0;
        sevenBitValue |= (internalValue & IntMaskByte2) << 1;
        sevenBitValue |= (internalValue & IntMaskByte3) << 2;
        sevenBitValue |= (internalValue & IntMaskByte4) << 3;

        return sevenBitValue;
    }

    // 4 bytes of 7-bits each. msbit is reset.
    private const uint SbMaskAll = 0x7F7F7F7F;
    private const uint SbMaskByte1 = 0x0000007F;
    private const uint SbMaskByte2 = 0x00007F00;
    private const uint SbMaskByte3 = 0x007F0000;
    private const uint SbMaskByte4 = 0x7F000000;

    private static uint ToInternal(uint sevenBitValue)
    {
        ValidateSevenBitValue(sevenBitValue);

        uint internalValue = (sevenBitValue & SbMaskByte1) >> 0;
        internalValue |= (sevenBitValue & SbMaskByte2) >> 1;
        internalValue |= (sevenBitValue & SbMaskByte3) >> 2;
        internalValue |= (sevenBitValue & SbMaskByte4) >> 3;

        return internalValue;
    }

    private static void ValidateSevenBitValue(uint sevenBitValue)
    {
        if ((sevenBitValue & ~SbMaskAll) > 0)
        {
            throw new OverflowException(
                $"The value is too large to be a seven bit uint: {sevenBitValue}");
        }
    }

    private static void ValidateInternalValue(uint internalValue)
    {
        if ((internalValue & ~IntMaskAll) > 0)
        {
            throw new OverflowException(
                $"The value is too large to convert to a seven bit uint: {internalValue}");
        }
    }
}
