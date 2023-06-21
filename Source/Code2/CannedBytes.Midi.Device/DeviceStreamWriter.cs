using System.IO;
using System.Text;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

public sealed class DeviceStreamWriter
{
    private readonly BitStreamWriter _bitWriter;

    internal DeviceStreamWriter(Stream stream, BitStreamWriter bitWriter)
    {
        Assert.IfArgumentNull(stream, nameof(stream));
        Assert.IfArgumentNull(bitWriter, nameof(bitWriter));

        BaseStream = stream;
        _bitWriter = bitWriter;
    }

    public Stream BaseStream { get; }

    public void WriteBitRange(ValueRange range, ushort value)
    {
        _bitWriter.WriteBits(BaseStream, range.Start, range.Length, value);
    }

    public void WriteStringAscii(string value, int byteLength)
    {
        var padded = value.PadRight(byteLength);
        var bytes = Encoding.ASCII.GetBytes(padded);
        BaseStream.Write(bytes, 0, byteLength);
    }

    public void Write(VarUInt64 value, BitOrder byteOrder = BitOrder.LittleEndian)
    {
        var length = ByteConverter.FromVarUInt64ToBytes(value, byteOrder, out var buffer);
        if (length > 0)
            BaseStream.Write(buffer, 0, length);
    }

    public void Flush()
    {
        _bitWriter.Flush(BaseStream);
        BaseStream.Flush();
    }
}
