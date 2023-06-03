using System;

namespace CannedBytes.Midi.Device.Converters
{
    partial class DataConverter
    {
        /// <summary>
        /// Temporary fake accessor to intercept a value.
        /// </summary>
        /// <typeparam name="ValueT">Data type the value is converted to.</typeparam>
        public class LogicalWriteAccessor<ValueT> : ILogicalWriteAccessor
        {
            public ValueT Value { get; set; }

            public int BitLength { get; set; }

            public bool Write<T>(T value, int bitLength)
                where T : IComparable
            {
                BitLength = bitLength;

                Value = (ValueT)Convert.ChangeType(value, typeof(ValueT));

                return true;
            }
        }

    }
}
