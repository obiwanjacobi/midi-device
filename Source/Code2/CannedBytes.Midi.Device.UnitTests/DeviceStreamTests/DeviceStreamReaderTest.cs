using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.DeviceStream;

public class DeviceStreamReaderTest
{
    private static DeviceStreamReader CreateDeviceStreamReader(byte[] buffer)
    {
        var stream = new MemoryStream(buffer, false);
        return new DeviceStreamReader(stream, new BitStreamReader());
    }

    [Fact]
    public void ReadBits()
    {
        var reader = CreateDeviceStreamReader(new byte[] { 0xFF });

        int result = reader.ReadBitRange(new Core.ValueRange(1, 5));

        result.Should().Be(31);
    }

    [Fact]
    public void ReadString()
    {
        const string expected = "Roland";
        var buffer = Encoding.ASCII.GetBytes(expected);
        var reader = CreateDeviceStreamReader(buffer);

        var actual = reader.ReadStringAscii(buffer.Length);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ReadVarUInt64()
    {
        var buffer = new byte[] { 0xA5, 0x5A, 0x6F };
        var reader = CreateDeviceStreamReader(buffer);

        var actual = reader.Read(buffer.Length);

        actual.TypeCode.Should().Be(Core.VarUInt64.VarTypeCode.UInt24);
        actual.ToUInt32().Should().Be(0x6F5AA5);
    }

    [Fact]
    public void ReadUInt48_BigEndian()
    {
        var buffer = new byte[] { 0xA5, 0x5A, 0x6F, 0x3B, 0x7E, 0xDD };
        var reader = CreateDeviceStreamReader(buffer);

        var actual = reader.ReadUInt48(Core.BitOrder.BigEndian);

        actual.Should().Be(0xA55A6F3B7EDD);
    }
}
