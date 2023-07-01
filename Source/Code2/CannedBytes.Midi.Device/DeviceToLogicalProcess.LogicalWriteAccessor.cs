using System;
using System.Runtime.InteropServices;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

partial class DeviceToLogicalProcess
{
    // helps in calling the right method on IMidiLogicalWriter
    internal sealed class LogicalWriteAccessor : ILogicalWriteAccessor
    {
        private readonly DeviceDataContext _context;
        private readonly IMidiLogicalWriter _writer;

        public LogicalWriteAccessor(DeviceDataContext context, IMidiLogicalWriter writer)
        {
            Assert.IfArgumentNull(context, nameof(context));
            Assert.IfArgumentNull(writer, nameof(writer));

            _context = context;
            _writer = writer;
        }

        // one method decides which one to use.
        public bool Write<T>(T value, int bitLength)
            where T : IComparable
        {
            var fieldValue = _context.FieldInfo.GetDataFieldValue<T>();

            // intercept the value and set it on the current Record Entry.
            if (_context.LogManager?.CurrentEntry is not null)
            {
                _context.LogManager.CurrentEntry.Data = value;
            }

            // validate against field constraints
            fieldValue.Validate(value);

            var devicePropName = _context.FieldInfo.CurrentField.Properties.DevicePropertyName;

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

            // no callback does not mean error
            return true;
        }

        private bool DispatchValue<T>(T value, int bitLength)
        {
            var type = typeof(T);

            if (bitLength == 0)
            {
                bitLength = DetermineBitLength(type);
            }

            var ctx = _context.CreateLogicalContext(bitLength);

            if (type == typeof(string))
            {
                return _writer.WriteString(ctx, (value as string)!);
            }

            if (bitLength == 1)
            {
                bool nativeValue = ConvertTo.ChangeType<T, bool>(value);

                return _writer.WriteBool(ctx, nativeValue);
            }
            else if (bitLength <= 8)
            {
                byte nativeValue = ConvertTo.ChangeType<T, byte>(value);

                return _writer.WriteByte(ctx, nativeValue);
            }
            else if (bitLength <= 16)
            {
                short nativeValue = ConvertTo.ChangeType<T, short>(value);

                return _writer.WriteShort(ctx, nativeValue);
            }
            else if (bitLength <= 32)
            {
                int nativeValue = ConvertTo.ChangeType<T, int>(value);

                return _writer.WriteInt(ctx, nativeValue);
            }
            else if (bitLength <= 64)
            {
                long nativeValue = ConvertTo.ChangeType<T, long>(value);

                return _writer.WriteLong(ctx, nativeValue);
            }

            throw new ArgumentException(
                    "The specified type is not supported: " + type.FullName);
        }

        private int DetermineBitLength(Type type)
        {
            int length = _context.FieldInfo.CurrentNode!.FieldConverterPair.Converter.ByteLength;

            if (length < 0)
            {
                return Marshal.SizeOf(type);
            }

            return length;
        }
    }
}