using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.InstancePathKeyTests;


public class InstancePathKeyTest
{
    [Fact]
    public void DefCtor_DepthAndIsZero_AllZero()
    {
        InstancePathKey key = new();

        key.Depth.Should().Be(0);
        key.IsZero.Should().BeTrue();
        key.Values.Should().BeEmpty();
        key.ToString().Should().BeEmpty();
    }

    [Fact]
    public void IndexCtor_DepthAndIsZero_AllOne()
    {
        InstancePathKey key = new(1);

        key.Depth.Should().Be(1);
        key.IsZero.Should().BeFalse();
        key.Values.Should().HaveCount(1);
        key.ToString().Should().Be("1");
    }

    [Fact]
    public void Add_DepthAndIsZero_CorrectValues()
    {
        InstancePathKey key = new();
        key.Add(0);
        key.Add(1);
        key.Add(0);

        key.Depth.Should().Be(3);
        key.IsZero.Should().BeFalse();
        key.Values.Should().HaveCount(3);
        key.ToString().Should().Be("0|1|0");
    }
}
