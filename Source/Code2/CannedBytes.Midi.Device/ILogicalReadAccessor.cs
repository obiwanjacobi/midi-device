using System;

namespace CannedBytes.Midi.Device
{
    public interface ILogicalReadAccessor
    {
        bool Read<T>(int bitLength, out T value)
            where T : IComparable;
    }
}
