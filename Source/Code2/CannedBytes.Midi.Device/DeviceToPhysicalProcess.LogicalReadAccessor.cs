using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

partial class DeviceToPhysicalProcess
{
    internal sealed class LogicalReadAccessor : ILogicalReadAccessor
    {
        private readonly DeviceDataContext _context;
        private readonly IMidiLogicalReader _reader;

        public LogicalReadAccessor(DeviceDataContext context, IMidiLogicalReader reader)
        {
            Assert.IfArgumentNull(context, nameof(context));
            Assert.IfArgumentNull(reader, nameof(reader));

            _context = context;
            _reader = reader;
        }

        public bool ReadString(out string value)
        {
            var ctx = _context.CreateLogicalContext(0);
            var success = _reader.ReadString(ctx, out var str);
            value = str;
            return success;
        }

        public bool ReadBits(int bitLength, out ushort value)
        {
            var ctx = _context.CreateLogicalContext(bitLength);

            if (bitLength == 1)
            {
                var r = _reader.ReadBool(ctx, out var b);
                value = ConvertTo.ChangeType<bool, ushort>(b);
                return r;
            }
            else if (bitLength <= 8)
            {
                var r = _reader.ReadByte(ctx, out var b);
                value = ConvertTo.ChangeType<byte, ushort>(b);
                return r;
            }
            else if (bitLength <= 16)
            {
                var r = _reader.ReadShort(ctx, out var s);
                value = ConvertTo.ChangeType<short, ushort>(s);
                return r;
            }

            throw new DeviceDataException(
                $"The specified bitLength is too big: {bitLength}.");
        }

        public bool Read(int byteLength, out VarUInt64 value)
        {
            if (byteLength < 1)
            {
                throw new DeviceDataException(
                    $"The specified byteLength {byteLength} is too small (< 1).");
            }
            if (byteLength > 8)
            {
                throw new DeviceDataException(
                    $"The specified byteLength {byteLength} is too big (> 8).");
            }

            var ctx = _context.CreateLogicalContext(byteLength * 8);

            if (byteLength == 1)
            {
                var r = _reader.ReadByte(ctx, out var b);
                value = b;
                return r;
            }
            if (byteLength == 2)
            {
                var r = _reader.ReadShort(ctx, out var s);
                value = s;
                return r;
            }
            else if (byteLength <= 4)
            {
                var r = _reader.ReadInt(ctx, out var i);
                value = i;
                return r;
            }
            else if (byteLength <= 8)
            {
                var r = _reader.ReadLong(ctx, out var l);
                value = (ulong)l;
                return r;
            }

            throw new DeviceDataException(
                "Internal Error in LogicalReadAccessor.Read().");
        }
    }
}
