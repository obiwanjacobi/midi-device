using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

public interface ILogicalReadAccessor
{
    bool ReadString(out string value);
    bool ReadBits(int bitLength, out ushort value);
    bool Read(int byteLength, out VarUInt64 value);
}
