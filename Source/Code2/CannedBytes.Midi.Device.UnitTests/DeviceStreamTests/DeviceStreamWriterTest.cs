using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.DeviceStream;

public class DeviceStreamWriterTest
{
    private static DeviceStreamWriter CreateDeviceStreamWriter(byte[] buffer)
    {
        var stream = new MemoryStream(buffer, true);
        return new DeviceStreamWriter(stream, new BitStreamWriter());
    }

    [Fact]
    public void WriteBits()
    {
        var buffer = new byte[1];
        var writer = CreateDeviceStreamWriter(buffer);

        writer.WriteBitRange(new Core.ValueRange(1, 5), 0xAA);
        writer.Flush();

        // 101(0_1010) => 0001_0100
        buffer[0].Should().Be(0b0001_0100);
    }

    [Fact]
    public void WriteString()
    {
        var length = 5; // too short on purpose
        var buffer = new byte[length];
        var writer = CreateDeviceStreamWriter(buffer);

        writer.WriteStringAscii("Roland", length);
        writer.Flush();

        var actual = Encoding.ASCII.GetString(buffer);
        actual.Should().Be("Rolan");
    }

    [Fact]
    public void WriteVarUInt64()
    {
        var buffer = new byte[4];
        var writer = CreateDeviceStreamWriter(buffer);

        writer.Write(0xA57E44);
        writer.Flush();

        buffer[0].Should().Be(0x44);
        buffer[1].Should().Be(0x7E);
        buffer[2].Should().Be(0xA5);
    }

    [Fact]
    public void WriteUInt48_BigEndian()
    {
        var buffer = new byte[8];
        var writer = CreateDeviceStreamWriter(buffer);

        writer.Write(0xA55A6F3B7EDD, Core.BitOrder.BigEndian);
        writer.Flush();

        buffer[0].Should().Be(0xA5);
        buffer[1].Should().Be(0x5A);
        buffer[2].Should().Be(0x6F);
        buffer[3].Should().Be(0x3B);
        buffer[4].Should().Be(0x7E);
        buffer[5].Should().Be(0xDD);
    }
}
