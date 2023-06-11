using System;

namespace CannedBytes.Midi.Core;

/// <summary>
/// A variable unsigend value of max 8 bytes.
/// </summary>
public struct VarUInt64
{
    private readonly ulong _value;

    /// <summary>
    /// Constructs 1 byte.
    /// </summary>
    /// <param name="value">The 8 bit value.</param>
    public VarUInt64(byte value)
    {
        _value = value;
        TypeCode = TypeCodeFor(_value);
        IsFixed = false;
    }

    /// <summary>
    /// Constructs 2 bytes.
    /// </summary>
    /// <param name="value">The 16-bit value.</param>
    public VarUInt64(ushort value)
    {
        _value = value;
        TypeCode = TypeCodeFor(_value);
        IsFixed = false;
    }

    /// <summary>
    /// Constructs 4 bytes.
    /// </summary>
    /// <param name="value">The 32-bit value.</param>
    public VarUInt64(uint value)
    {
        _value = value;
        TypeCode = TypeCodeFor(_value);
        IsFixed = false;
    }

    /// <summary>
    /// Constructs 8 bytes.
    /// </summary>
    /// <param name="value">The 64-bit value.</param>
    public VarUInt64(ulong value)
    {
        _value = value;
        TypeCode = TypeCodeFor(_value);
        IsFixed = false;
    }

    /// <summary>
    /// Constructs a specific byte size.
    /// </summary>
    /// <param name="value">The variable value.</param>
    /// <param name="typeCode">Indication of the variable size.</param>
    public VarUInt64(ulong value, VarTypeCode typeCode)
    {
        _value = value;
        TypeCode = typeCode;
        IsFixed = true;
    }

    /// <summary>
    /// Indicates if the size can be changed in-place.
    /// </summary>
    public bool IsFixed { get; }

    /// <summary>
    /// Indicates if this instance is initialized to a value.
    /// </summary>
    public bool IsEmpty
    {
        get { return TypeCode == VarTypeCode.Empty; }
    }

    /// <summary>
    /// Gets the actual size of this instance.
    /// </summary>
    public VarTypeCode TypeCode { get; }

    public bool Equals(VarUInt64 value)
    {
        return Equals(value._value);
    }

    public bool Equals(ulong value)
    {
        return _value == value;
    }

    public override bool Equals(object obj)
    {
        if (obj is VarUInt64 var64)
        {
            return Equals(var64);
        }
        if (obj is byte b)
        {
            return Equals((ulong)b);
        }
        if (obj is ushort s)
        {
            return Equals((ulong)s);
        }
        if (obj is uint ui)
        {
            return Equals((ulong)ui);
        }
        if (obj is ulong ul)
        {
            return Equals(ul);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    /// <summary>
    /// Creates a new instance for the specified <paramref name="typeCode"/>.
    /// </summary>
    /// <param name="typeCode">The desired size. May cause value truncation.</param>
    /// <returns>Returns a new instance.</returns>
    public VarUInt64 ConvertTo(VarTypeCode typeCode)
    {
        var value = MaskValue(typeCode);

        return new VarUInt64(value, typeCode);
    }

    /// <summary>
    /// Returns a 1 byte value.
    /// </summary>
    /// <returns>Returns the 8-bit value.</returns>
    /// <exception cref="InvalidCastException">Thrown when the <see cref="TypeCode"/> is too large.</exception>
    public byte ToByte()
    {
        ThrowIfInvalidCast(VarTypeCode.UInt8);

        return (byte)_value;
    }

    /// <summary>
    /// Returns a 2 byte value.
    /// </summary>
    /// <returns>Returns the 16-bit value.</returns>
    /// <exception cref="InvalidCastException">Thrown when the <see cref="TypeCode"/> is too large.</exception>
    public ushort ToUInt16()
    {
        ThrowIfInvalidCast(VarTypeCode.UInt16);

        return (ushort)_value;
    }

    /// <summary>
    /// Returns a 4 byte value.
    /// </summary>
    /// <returns>Returns the 32-bit value.</returns>
    /// <exception cref="InvalidCastException">Thrown when the <see cref="TypeCode"/> is too large.</exception>
    public uint ToUInt32()
    {
        ThrowIfInvalidCast(VarTypeCode.UInt32);

        return (uint)_value;
    }

    /// <summary>
    /// Returns a 8 byte value.
    /// </summary>
    /// <returns>Returns the 64-bit value.</returns>
    /// <exception cref="InvalidCastException">Thrown when the <see cref="TypeCode"/> is too large.</exception>
    public ulong ToUInt64()
    {
        ThrowIfInvalidCast(VarTypeCode.UInt64);

        return _value;
    }

    /// <summary>
    /// Formats a hex and dec value.
    /// </summary>
    /// <returns>Never returns null or empty.</returns>
    public override string ToString()
    {
        return _value.ToString() + " (" + TypeCode.ToString() + ")";
    }

    public string ToString(string format)
    {
        return _value.ToString(format);
    }

    #region Operators

    public static bool operator ==(VarUInt64 thisValue, VarUInt64 thatValue)
    {
        return thisValue.Equals(thatValue);
    }

    public static bool operator ==(VarUInt64 thisValue, byte thatValue)
    {
        return thisValue.Equals((ulong)thatValue);
    }

    public static bool operator ==(VarUInt64 thisValue, ushort thatValue)
    {
        return thisValue.Equals((ulong)thatValue);
    }

    public static bool operator ==(VarUInt64 thisValue, uint thatValue)
    {
        return thisValue.Equals((ulong)thatValue);
    }

    public static bool operator ==(VarUInt64 thisValue, ulong thatValue)
    {
        return thisValue.Equals(thatValue);
    }

    public static bool operator !=(VarUInt64 thisValue, VarUInt64 thatValue)
    {
        return !thisValue.Equals(thatValue);
    }

    public static bool operator !=(VarUInt64 thisValue, byte thatValue)
    {
        return !thisValue.Equals((ulong)thatValue);
    }

    public static bool operator !=(VarUInt64 thisValue, ushort thatValue)
    {
        return !thisValue.Equals((ulong)thatValue);
    }

    public static bool operator !=(VarUInt64 thisValue, uint thatValue)
    {
        return !thisValue.Equals((ulong)thatValue);
    }

    public static bool operator !=(VarUInt64 thisValue, ulong thatValue)
    {
        return !thisValue.Equals(thatValue);
    }

    public static VarUInt64 operator +(VarUInt64 left, VarUInt64 right)
    {
        return CreateNewFrom(left, left._value + right._value);
    }

    public static VarUInt64 operator +(VarUInt64 left, byte right)
    {
        return CreateNewFrom(left, left._value + right);
    }

    public static VarUInt64 operator +(VarUInt64 left, ushort right)
    {
        return CreateNewFrom(left, left._value + right);
    }

    public static VarUInt64 operator +(VarUInt64 left, uint right)
    {
        return CreateNewFrom(left, left._value + right);
    }

    public static VarUInt64 operator +(VarUInt64 left, ulong right)
    {
        return CreateNewFrom(left, left._value + right);
    }

    public static VarUInt64 operator -(VarUInt64 left, VarUInt64 right)
    {
        return new VarUInt64(left._value - right._value);
    }

    public static VarUInt64 operator -(VarUInt64 left, byte right)
    {
        return new VarUInt64(left._value - right);
    }

    public static VarUInt64 operator -(VarUInt64 left, ushort right)
    {
        return new VarUInt64(left._value - right);
    }

    public static VarUInt64 operator -(VarUInt64 left, uint right)
    {
        return new VarUInt64(left._value - right);
    }

    public static VarUInt64 operator -(VarUInt64 left, ulong right)
    {
        return new VarUInt64(left._value - right);
    }

    public static implicit operator byte(VarUInt64 value)
    {
        return value.ToByte();
    }

    public static implicit operator ushort(VarUInt64 value)
    {
        return value.ToUInt16();
    }

    public static implicit operator uint(VarUInt64 value)
    {
        return value.ToUInt32();
    }

    public static implicit operator ulong(VarUInt64 value)
    {
        return value.ToUInt64();
    }

    public static implicit operator VarUInt64(byte value)
    {
        return new VarUInt64(value);
    }
    
    public static implicit operator VarUInt64(ushort value)
    {
        return new VarUInt64(value);
    }

    public static implicit operator VarUInt64(uint value)
    {
        return new VarUInt64(value);
    }

    public static implicit operator VarUInt64(ulong value)
    {
        return new VarUInt64(value);
    }

    public static implicit operator VarUInt64(int value)
    {
        return new VarUInt64((uint)value);
    }

    #endregion

    /// <summary>
    /// Returns a native .NET type for a specific size.
    /// </summary>
    /// <param name="typeCode">An indication of the requested size.</param>
    /// <returns>Returns byte, ushort, uint or ulong types.</returns>
    public static Type GetTypeFor(VarTypeCode typeCode)
    {
        Type result = null;

        switch (typeCode)
        {
            case VarTypeCode.UInt8:
                result = typeof(byte);
                break;
            case VarTypeCode.UInt16:
                result = typeof(ushort);
                break;
            case VarTypeCode.UInt24:
            case VarTypeCode.UInt32:
                result = typeof(uint);
                break;
            case VarTypeCode.UInt40:
            case VarTypeCode.UInt48:
            case VarTypeCode.UInt56:
            case VarTypeCode.UInt64:
                result = typeof(ulong);
                break;
        }

        return result;
    }

    /// <summary>
    /// Returns the smallest size for a specific value.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <returns>Returns the size indication.</returns>
    public static VarTypeCode TypeCodeFor(ulong value)
    {
        for (VarTypeCode typeCode = 0; typeCode < VarTypeCode.UInt64; typeCode++)
        {
            if (MaxValueFor(typeCode) >= value)
            {
                return typeCode;
            }
        }

        throw new InvalidOperationException(
            $"Internal Error: could not find the VarTypeCode for: {value}");
    }

    /// <summary>
    /// Returns the max-value for a specific size.
    /// </summary>
    /// <param name="typeCode">The size indication.</param>
    /// <returns>Returns the max unsigned value.</returns>
    public static ulong MaxValueFor(VarTypeCode typeCode)
    {
        ulong maxValue = 0;

        switch (typeCode)
        {
            case VarTypeCode.UInt8:
                maxValue = byte.MaxValue;
                break;
            case VarTypeCode.UInt16:
                maxValue = ushort.MaxValue;
                break;
            case VarTypeCode.UInt24:
                maxValue = 0xFFFFFF;
                break;
            case VarTypeCode.UInt32:
                maxValue = UInt32.MaxValue;
                break;
            case VarTypeCode.UInt40:
                maxValue = 0xFFFFFFFFFF;
                break;
            case VarTypeCode.UInt48:
                maxValue = 0xFFFFFFFFFFFF;
                break;
            case VarTypeCode.UInt56:
                maxValue = 0xFFFFFFFFFFFFFF;
                break;
            case VarTypeCode.UInt64:
                maxValue = ulong.MaxValue;
                break;
        }

        return maxValue;
    }

    private static VarUInt64 CreateNewFrom(VarUInt64 origin, ulong value)
    {
        if (origin.IsFixed)
        {
            VarTypeCode typeCode = TypeCodeFor(value);

            if (origin.TypeCode < typeCode)
            {
                throw new OverflowException(
                    "The result will overflow the fixed VarUInt64.");
            }

            return new VarUInt64(value, typeCode);
        }

        return new VarUInt64(value);
    }

    private ulong MaskValue(VarTypeCode typeCode)
    {
        var mask = MaxValueFor(typeCode);

        return _value & mask;
    }

    private void ThrowIfInvalidCast(VarTypeCode typeCode)
    {
        if (TypeCode > typeCode)
        {
            throw new InvalidCastException(
                $"The value '{_value}' can not be cast to type code '{typeCode}'. It is too big '{TypeCode}'.");
        }
    }

    public static readonly VarUInt64 Zero = new();

    /// <summary>
    /// The variable sizes supported by the VarUint64 class.
    /// </summary>
    public enum VarTypeCode
    {
        /// <summary>Not initialized, or unable to determine size.</summary>
        Empty,

        /// <summary>1 byte, 8 bits.</summary>
        UInt8,

        /// <summary>2 bytes, 16 bits.</summary>
        UInt16,

        /// <summary>3 bytes, 24 bits.</summary>
        UInt24,

        /// <summary>4 bytes, 32 bits.</summary>
        UInt32,

        /// <summary>5 bytes, 40 bits.</summary>
        UInt40,

        /// <summary>6 bytes, 48 bits.</summary>
        UInt48,

        /// <summary>7 bytes, 56 bits.</summary>
        UInt56,

        /// <summary>8 bytes, 64 bits.</summary>
        UInt64
    }
}
