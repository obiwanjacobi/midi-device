using System;

namespace CannedBytes.Midi.Device.Message
{
    public struct SevenBitUInt32 : IConvertible
    {
        public const int MinValue = 0;
        public const int MaxValue = 0x7F7F7F7F;
        public const byte MinByteValue = 0;
        public const byte MaxByteValue = 0x7F;
        private const int MaxBytes = 4;

        public SevenBitUInt32(int value)
        {
            Check.IfArgumentOutOfRange(value, MinValue, MaxValue, "value");

            this.bytes = null;
            this.bytes = this.Add(ToBytes(value));
        }

        public SevenBitUInt32(byte byte3, byte byte2, byte byte1, byte byte0)
        {
            Check.IfArgumentOutOfRange(byte3, MinByteValue, MaxByteValue, "byte3");
            Check.IfArgumentOutOfRange(byte2, MinByteValue, MaxByteValue, "byte2");
            Check.IfArgumentOutOfRange(byte1, MinByteValue, MaxByteValue, "byte1");
            Check.IfArgumentOutOfRange(byte0, MinByteValue, MaxByteValue, "byte0");

            this.bytes = new byte[MaxBytes];
            this.bytes[0] = byte0;
            this.bytes[1] = byte1;
            this.bytes[2] = byte2;
            this.bytes[3] = byte3;
        }

        private SevenBitUInt32(byte[] bytes)
        {
            this.bytes = bytes;
        }

        private byte[] bytes;

        public byte Byte0
        { get { return this.bytes[0]; } }

        public byte Byte1
        { get { return this.bytes[1]; } }

        public byte Byte2
        { get { return this.bytes[2]; } }

        public byte Byte3
        { get { return this.bytes[3]; } }

        public byte[] ToBytes()
        {
            return this.bytes;
        }

        public int ToInt32()
        {
            if (this.bytes == null)
            {
                return 0;
            }

            return (int)this.bytes[3] << 24 | (int)this.bytes[2] << 16 | (int)this.bytes[1] << 8 | (int)this.bytes[0];
        }

        public override string ToString()
        {
            return ToString("X");
        }

        public string ToString(string format)
        {
            var value = ToInt32();

            return value.ToString(format);
        }

        public override int GetHashCode()
        {
            return ToInt32().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SevenBitUInt32)
            {
                return Equals((SevenBitUInt32)obj);
            }

            if (obj is int)
            {
                return this.ToInt32() == (int)obj;
            }

            if (obj is long)
            {
                return this.ToInt32() == (int)obj;
            }

            return base.Equals(obj);
        }

        public bool Equals(SevenBitUInt32 value)
        {
            return this.ToInt32() == value.ToInt32();
        }

        private byte[] Add(byte[] bytes)
        {
            var thisValue = this.bytes;

            if (bytes == null)
            {
                bytes = Empty.bytes;
            }
            if (thisValue == null)
            {
                thisValue = Empty.bytes;
            }

            return Add(thisValue, bytes);
        }

        private static byte[] Add(byte[] thisValue, byte[] thatValue)
        {
            var result = new byte[MaxBytes];

            int carry = 0;
            for (int i = 0; i < MaxBytes; i++)
            {
                int sum = (int)thisValue[i] + (int)thatValue[i] + carry;
                carry = sum / (MaxByteValue + 1);
                result[i] = (byte)(sum % (MaxByteValue + 1));
            }

            if (carry > 0)
            {
                throw new OverflowException();
            }

            return result;
        }

        private SevenBitUInt32 Sub(int value)
        {
            var result = this.ToInt32() - value;
            return new SevenBitUInt32(result & MaxValue);
        }

        public static readonly SevenBitUInt32 Empty = new SevenBitUInt32(0, 0, 0, 0);

        private static byte[] ToBytes(int value)
        {
            var bytes = new byte[MaxBytes];
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);

            return bytes;
        }

        public static SevenBitUInt32 operator +(SevenBitUInt32 sbInt, SevenBitUInt32 value)
        {
            return new SevenBitUInt32(sbInt.Add(value.bytes));
        }

        public static SevenBitUInt32 operator +(SevenBitUInt32 sbInt, int value)
        {
            return new SevenBitUInt32(sbInt.Add(ToBytes(value)));
        }

        public static SevenBitUInt32 operator -(SevenBitUInt32 sbInt, SevenBitUInt32 value)
        {
            return sbInt.Sub(value.ToInt32());
        }

        public static SevenBitUInt32 operator -(SevenBitUInt32 sbInt, int value)
        {
            return sbInt.Sub(value);
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

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return this.ToInt32() != 0;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte)this.ToInt32();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return (char)this.ToInt32();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return (decimal)this.ToInt32();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)this.ToInt32();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short)this.ToInt32();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return this.ToInt32();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)this.ToInt32();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte)this.ToInt32();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float)this.ToInt32();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return this.ToString("X");
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotSupportedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort)this.ToInt32();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint)this.ToInt32();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)this.ToInt32();
        }
    }
}