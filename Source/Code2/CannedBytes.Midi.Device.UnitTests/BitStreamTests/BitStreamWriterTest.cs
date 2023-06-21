using System.IO;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.UnitTests.BitStreamTests;

public class BitStreamWriterTest
{
    private readonly ITestOutputHelper _output;

    public BitStreamWriterTest(ITestOutputHelper output)
        => _output = output;

    private Stream NewStream(params byte[] bytes)
        => new MemoryStream(bytes, writable: true);

    [Fact]
    public void WriteBits_LoNibble_Twice()
    {
        var writer = new BitStreamWriter();
        var buffer = new byte[2];
        var stream = NewStream(buffer);

        writer.WriteBits(stream, 1, 3, 0xAA);
        writer.WriteBits(stream, 1, 3, 0x55);
        writer.Flush(stream);

        // 1010_1(010)
        buffer[0].Should().Be(0b0100);
        // 0101_0(101)
        buffer[1].Should().Be(0b1010);
    }

    [Fact]
    public void WriteBits_LoNibble_HiNibble()
    {
        var writer = new BitStreamWriter();
        var buffer = new byte[2];
        var stream = NewStream(buffer);

        writer.WriteBits(stream, 1, 3, 0xAA);
        writer.WriteBits(stream, 6, 5, 0x55);
        writer.Flush(stream);

        // 1010_1(010)
        // 010(1_01)(01) => lo-part goes in [0] starting at bit6 rest in [1]
        buffer[0].Should().Be(0b_01_000100);
        buffer[1].Should().Be(0b101);
    }

    [Fact]
    public void WriteBits_LoAndHiNibble_From_LoAndHiByte()
    {
        var writer = new BitStreamWriter();
        var buffer = new byte[2];
        var stream = NewStream(buffer);

        writer.WriteBits(stream, 1, 3, 0xAA);
        writer.WriteBits(stream, 12, 3, 0x55);
        writer.Flush(stream);

        // 1010_1(010)
        buffer[0].Should().Be(0b0100);
        // 01010_0(101)
        buffer[1].Should().Be(0b0101_0000);
    }
}