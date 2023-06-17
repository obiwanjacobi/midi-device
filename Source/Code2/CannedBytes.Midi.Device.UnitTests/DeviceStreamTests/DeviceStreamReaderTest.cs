using System.IO;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.DeviceStream;


public class DeviceStreamReaderTest
{
    private static DeviceStreamReader CreateDeviceStreamReader(byte[] buffer)
    {
        var stream = new MemoryStream(buffer, false);
        return new DeviceStreamReader(stream, new Carry());
    }

    [Fact]
    public void ReadWithCarry()
    {
        var reader = CreateDeviceStreamReader(new byte[] { 0xFF });

        int read = reader.Read(BitFlags.Bit1 | BitFlags.Bit2 | BitFlags.Bit3 | BitFlags.Bit4 | BitFlags.Bit5, out ushort result);
        read.Should().BeGreaterThan(0);

        // value is **not** shifted down!
        result.Should().Be(62);
    }
}
