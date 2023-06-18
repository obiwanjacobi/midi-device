using System;
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

        public bool Read<T>(int bitLength, out T value) where T : IComparable
        {
            throw new NotImplementedException();
        }
    }
}
