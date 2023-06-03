using System;
using System.Runtime.InteropServices;

namespace CannedBytes.Midi.Device
{
    // helps in calling the right method on IMidiLogicalWriter
    public class LogicalWriteAccessor : ILogicalWriteAccessor
    {
        private DeviceDataContext _context;
        private IMidiLogicalWriter _writer;

        public LogicalWriteAccessor(DeviceDataContext context, IMidiLogicalWriter writer)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(writer, "writer");

            _context = context;
            _writer = writer;
        }

        // one method decides which one to use.
        public bool Write<T>(T value, int bitLength)
            where T : IComparable
        {
            var fieldValue = _context.FieldInfo.GetDataFieldValue<T>();

            // intercept the value and set it on the current Record Entry.
            if (_context.RecordManager != null &&
                _context.RecordManager.CurrentEntry != null)
            {
                _context.RecordManager.CurrentEntry.Data = value;
            }

            // validate against field constraints
            fieldValue.Validate(value);

            var devicePropName = _context.FieldInfo.CurrentField.ExtendedProperties.DevicePropertyName;
            
            // write device property value
            if (!String.IsNullOrEmpty(devicePropName))
            {
                _context.DeviceProperties.Add(
                    _context.FieldInfo.CurrentField.Name.SchemaName, devicePropName, value);
            }

            if (fieldValue.Callback)
            {
                return DispatchValue<T>(value, bitLength);
            }

            return false;
        }

        protected virtual bool DispatchValue<T>(T value, int bitLength)
        {
            var type = typeof(T);
            var ctx = CreateLogicalContext();

            if (type == typeof(string))
            {
                return _writer.WriteString(ctx, value as string);
            }

            if (bitLength == 0)
            {
                bitLength = DetermineBitLength(type);
            }

            ctx.BitLength = bitLength;

            if (bitLength == 1)
            {
                var nativeValue = (bool)Convert.ChangeType(value, typeof(bool));

                return _writer.WriteBool(ctx, nativeValue);
            }
            else if (bitLength <= 8)
            {
                var nativeValue = (byte)Convert.ChangeType(value, typeof(byte));

                return _writer.WriteByte(ctx, nativeValue);
            }
            else if (bitLength <= 16)
            {
                var nativeValue = (short)Convert.ChangeType(value, typeof(short));

                return _writer.WriteShort(ctx, nativeValue);
            }
            else if (bitLength <= 32)
            {
                var nativeValue = (int)Convert.ChangeType(value, typeof(int));

                return _writer.WriteInt(ctx, nativeValue);
            }
            else if (bitLength <= 64)
            {
                var nativeValue = (long)Convert.ChangeType(value, typeof(long));

                return _writer.WriteLong(ctx, nativeValue);
            }

            throw new ArgumentException(
                    "The specified type is not supported: " + type.FullName);
        }

        private LogicalContext CreateLogicalContext()
        {
            var ctx = _context.CreateLogicalContext();

            return ctx;
        }

        private int DetermineBitLength(Type type)
        {
            var length = _context.FieldInfo.CurrentNode.FieldConverterPair.Converter.ByteLength;

            if (length < 0)
            {
                return Marshal.SizeOf(type);
            }

            return length;
        }
    }
}
