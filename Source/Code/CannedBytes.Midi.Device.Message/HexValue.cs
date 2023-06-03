using System;
using System.Globalization;

namespace CannedBytes.Midi.Device.Message
{
    public class HexValue
    {
        public HexValue(string value)
        {
            Value = Parse(value);
        }

        protected SevenBitUInt32 Parse(string value)
        {
            int hex = 0;

            if (!int.TryParse(value, out hex))
            {
                var parts = value.Split(' ', '-');
                var bytes = new byte[4];

                for (int i = 0; i < parts.Length; i++)
                {
                    bytes[i] = byte.Parse(parts[i], NumberStyles.HexNumber);
                }

                Array.Reverse(bytes, 0, parts.Length);

                return new SevenBitUInt32(bytes[3], bytes[2], bytes[1], bytes[0]);
            }

            return new SevenBitUInt32(hex);
        }

        public SevenBitUInt32 Value { get; protected set; }
    }
}