using System.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.DeviceTests.CarryTest;

public class BitStreamReaderTest
{
    private readonly ITestOutputHelper _output;

    public BitStreamReaderTest(ITestOutputHelper output)
        => _output = output;

    private Stream NewStream(params byte[] bytes)
        => new MemoryStream(bytes, writable: false);

    [Fact]
    public void ReadBits_Bit1_Len3()
    {
        var reader = new BitStreamReader();
        var stream = NewStream(0b1110_0101, 0b1011_1011);

        var value = reader.ReadBits(stream, 1, 3);
        value.Should().Be(0b010);

        value = reader.ReadBits(stream, 1, 3);
        value.Should().Be(0b101);
    }

    [Fact]
    public void ReadBits_Bit6_Len5()
    {
        var reader = new BitStreamReader();
        var stream = NewStream(0b1110_0101, 0b1011_1011);

        var value = reader.ReadBits(stream, 1, 3);
        value.Should().Be(0b010);

        value = reader.ReadBits(stream, 6, 5);
        value.Should().Be(0b01111);
    }
}