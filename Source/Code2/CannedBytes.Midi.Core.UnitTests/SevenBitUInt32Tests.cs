using System;
using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Core.UnitTests;

public class SevenBitUInt32Tests
{
    [Fact]
    public void Ctor_ParseHex7BitValue_ToUInt32ReflectsValue()
    {
        var sb = new SevenBitUInt32("20-10-08-04H");

        sb.ToUInt32().Should().Be(0x20100804);
    }

    [Fact]
    public void Ctor_ParseDec7BitValue_ToUInt32ReflectsValue()
    {
        var sb = new SevenBitUInt32("20-10-08-04");

        sb.ToUInt32().Should().Be(336201732);
    }

    [Fact]
    public void Ctor_ParseInternalDec_ToUInt32ReflectsValue()
    {
        var sb = new SevenBitUInt32("20100804");

        sb.ToUInt32().Should().Be(20067908);
    }

    [Fact]
    public void FromSevenBit_7BitValue_ToUInt32ReflectsValue()
    {
        uint value = 0x1000;

        var sb = SevenBitUInt32.FromSevenBitValue(value);

        sb.ToUInt32().Should().Be(value);
    }

    [Fact]
    public void FromInt32_InternalValue_ToUInt32ReflectsValue()
    {
        int value = 0x1080;

        var sb = SevenBitUInt32.FromInt32(value);

        sb.ToUInt32().Should().Be(0x2100);
    }

    [Fact]
    public void FromSevenBit_Invalid7BitValue_ThrowsException()
    {
        uint value = 0x1080;

        Action act = () => SevenBitUInt32.FromSevenBitValue(value);

        act.Should().Throw<OverflowException>();
    }

    [Fact]
    public void FromSevenBit_InvalidInternalValue_ThrowsException()
    {
        int value = -1;

        Action act = () => SevenBitUInt32.FromInt32(value);

        act.Should().Throw<OverflowException>();
    }
}
