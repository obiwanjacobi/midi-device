﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CannedBytes.Midi.Core
{
    public static class ValueParser
    {
        public static int ParseInt32(string s)
        {
            int value;

            if (!TryParseInt32(s, out value))
            {
                throw new FormatException(
                    "String could not be parsed into an Int32: " + s);
            }

            return value;
        }

        public static bool TryParseInt32(string s, out int value)
        {
            value = 0;

            bool isHex = IsHexadecimal(ref s);
            NumberStyles numberStyle = isHex ? NumberStyles.HexNumber : NumberStyles.Number;

            s = s.Replace("-", String.Empty).Replace(" ", String.Empty);
 
            return Int32.TryParse(s, numberStyle, CultureInfo.InvariantCulture, out value);
        }

        public static long ParseInt64(string s)
        {
            long value;

            if (!TryParseInt64(s, out value))
            {
                throw new FormatException(
                    "String could not be parsed into an Int64: " + s);
            }

            return value;
        }

        public static bool TryParseInt64(string s, out long value)
        {
            value = 0;

            bool isHex = IsHexadecimal(ref s);
            NumberStyles numberStyle = isHex ? NumberStyles.HexNumber : NumberStyles.Number;

            s = s.Replace("-", String.Empty).Replace(" ", String.Empty);

            return Int64.TryParse(s, numberStyle, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParseToBytes(string s, Ordering ordering, out byte[] bytes)
        {
            if (String.IsNullOrEmpty(s))
            {
                bytes = new byte[] { };
                return true;
            }

            bytes = null;
            byte[] results = null;
            string[] values;

            bool isHex = IsHexadecimal(ref s);
            NumberStyles numberStyle = isHex ? NumberStyles.HexNumber : NumberStyles.Number;

            if (TrySplitOnSeparators(s, out values))
            {
                if (!TryParseToBytes(values, numberStyle, out results))
                {
                    return false;
                }
            }
            else
            {
                long parsed;

                if (!Int64.TryParse(s, numberStyle, CultureInfo.InvariantCulture, out parsed))
                {
                     return false;
                }

                results = ByteConverter.FromUInt64ToBytes((ulong)parsed, Ordering.BigEndian);
            }

            if (ordering == Ordering.LittleEndian)
            {
                Array.Reverse(results);
            }

            bytes = results;
            return true;
        }

        private static bool TryParseToBytes(string[] values, NumberStyles numberStyle, out byte[] bytes)
        {
            bytes = null;
            byte[] result = null;
            int index = 0;

            if (values.Length > 1)
            {
                result = new byte[values.Length];

                foreach (var value in values)
                {
                    byte parsed;

                    if (!byte.TryParse(value, numberStyle, CultureInfo.InvariantCulture, out parsed))
                    {
                        return false;
                    }

                    result[index] = parsed;
                    index++;
                }

                bytes = result;
                return true;
            }

            return false;
        }

        private static bool IsHexadecimal(ref string s)
        {
            s = s.Trim();

            if (s.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                s = s.Substring(0, s.Length - 1);
                return true;
            }

            return false;
        }

        private static bool TrySplitOnSeparators(string s, out string[] parts)
        {
            s = s.Trim();
            parts = null;

            var values = s.Split('-', ' ');

            if (values.Length > 1)
            {
                // check part length
                foreach (var value in values)
                {
                    if (value.Length > 2)
                    {
                        return false;
                    }
                }

                parts = values;
                return true;
            }

            // one element
            return false;
        }
    }
}
