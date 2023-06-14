using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Core.UnitTests;

public class ByteConverterTests
{
    [Fact]
    public void FromUint32ToSevenBitBytes_7BitValueBE_RoundtripSameValue()
    {
        const uint value = (uint)0x08040201;
        byte[] bytes = ByteConverter.FromUint32ToSevenBitBytes(value, ByteOrder.BigEndian);

        bytes.Should().NotBeNull();
        bytes.Should().HaveCount(4);

        uint result = ByteConverter.FromSevenBitBytesToUInt32(bytes, ByteOrder.BigEndian);

        result.Should().Be(value);
    }

    [Fact]
    public void FromUint64ToBytes_ValueBE_RoundtripSameValue()
    {
        const ulong value = (ulong)0x7040201008040201;
        byte[] bytes = ByteConverter.FromUInt64ToBytes(value, ByteOrder.BigEndian);

        bytes.Should().NotBeNull();
        bytes.Should().HaveCount(8);

        long result = ByteConverter.FromBytesToInt64(bytes, ByteOrder.BigEndian);

        result.Should().Be((long)value);
    }

    [Fact]
    public void FromUint32ToSevenBitBytes_7BitValueLE_RoundtripSameValue()
    {
        const uint value = (uint)0x08040201;
        byte[] bytes = ByteConverter.FromUint32ToSevenBitBytes(value, ByteOrder.LittleEndian);

        bytes.Should().NotBeNull();
        bytes.Should().HaveCount(4);

        uint result = ByteConverter.FromSevenBitBytesToUInt32(bytes, ByteOrder.LittleEndian);

        result.Should().Be(value);
    }

    [Fact]
    public void FromUint64ToBytes_ValueLE_RoundtripSameValue()
    {
        const ulong value = (ulong)0x7040201008040201;
        byte[] bytes = ByteConverter.FromUInt64ToBytes(value, ByteOrder.LittleEndian);

        bytes.Should().NotBeNull();
        bytes.Should().HaveCount(8);

        long result = ByteConverter.FromBytesToInt64(bytes, ByteOrder.LittleEndian);

        result.Should().Be((long)value);
    }
}
