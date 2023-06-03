using System;

namespace CannedBytes.Midi.Device
{
    public interface ILogicalWriteAccessor
    {
        bool Write<T>(T value, int bitLength)
            where T : IComparable;
    }
}
