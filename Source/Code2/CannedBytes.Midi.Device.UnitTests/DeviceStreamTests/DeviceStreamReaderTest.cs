using System.IO;
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
    public void ReadWithCarry()
    {
        var reader = CreateDeviceStreamReader(new byte[] { 0xFF });

        int result = reader.ReadBitRange(new Core.ValueRange(1, 5));

        result.Should().Be(31);
    }
}
