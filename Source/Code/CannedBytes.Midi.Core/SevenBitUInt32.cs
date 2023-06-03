using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannedBytes.Midi.Core
{
    public struct SevenBitUInt32 : IConvertible
    {
        private uint _value;

        /// <summary>
        /// conventional uint value
        /// </summary>
        /// <param name="internalValue"></param>
        private SevenBitUInt32(uint internalValue)
        {
            ValidateInternalValue(internalValue);

            _value = internalValue;
        }

        /// <summary>
        /// parse ctor
        /// </summary>
        /// <param name="sevenBitValueString"></param>
        public SevenBitUInt32(string sevenBitValueString)
        {
            _value = 0;
            Parse(sevenBitValueString);
        }

        /// <summary>
        /// Makes any conventional value a seven bit uint. Will throw exceptions if value is too large.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SevenBitUInt32 FromInt32(int value)
        {
            return new SevenBitUInt32((uint)value);
        }

        public static SevenBitUInt32 FromSevenBitValue(uint value)
        {
            return new SevenBitUInt32(ToInternal(value));
        }

        public static SevenBitUInt32 FromString(string s)
        {
            return new SevenBitUInt32(s);
        }

        public void Parse(string s)
        {
            if (!ParseInternal(s))
            {
                throw new FormatException(
                    "The string could not be parsed into a SeventBitUInt32: " + s);
            }
        }

        public static bool TryParse(string s, out SevenBitUInt32 value)
        {
            value = new SevenBitUInt32();

            return value.ParseInternal(s);
        }

        private bool ParseInternal(string s)
        {
            byte[] bytes;
            var success = ValueParser.TryParseToBytes(s, Ordering.LittleEndian, out bytes);

            if (success)
            {
                this.Bytes = bytes;
            }

            return success;
        }

        public static readonly SevenBitUInt32 Zero = new SevenBitUInt32((uint)0);

        public int ToInt32()
        {
            return (int)ToUInt32();
        }

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
            set { _value = ByteConverter.FromSevenBitBytesToUInt32(value, Ordering.LittleEndian); }
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

        public override string ToString()
        {
            return this.ToString("X") + "H";
        }

        public string ToString(string format)
        {
            var value = ToUInt32();

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

            var sevenBitValue = (internalValue & IntMaskByte1) << 0;
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

            var internalValue = (sevenBitValue & SbMaskByte1) >> 0;
            internalValue |= (sevenBitValue & SbMaskByte2) >> 1;
            internalValue |= (sevenBitValue & SbMaskByte3) >> 2;
            internalValue |= (sevenBitValue & SbMaskByte4) >> 3;

            return internalValue;
        }

        private static void ValidateSevenBitValue(uint sevenBitValue)
        {
            if ((sevenBitValue & ~SbMaskAll) > 0)
            {
                throw new OverflowException("The value is too large to be a seven bit uint: " + sevenBitValue);
            }
        }

        private static void ValidateInternalValue(uint internalValue)
        {
            if ((internalValue & ~IntMaskAll) > 0)
            {
                throw new ArgumentException(
                    "The value is too large to convert to a seven bit uint: " + internalValue, "internalValue");
            }
        }
    }
}
