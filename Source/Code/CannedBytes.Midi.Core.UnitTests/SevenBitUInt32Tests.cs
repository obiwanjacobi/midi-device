using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Core.UnitTests
{
    
    public class SevenBitUInt32Tests
    {
        [Fact]
        public void Ctor_ParseHex()
        {
            var sb = new SevenBitUInt32("20-10-08-04H");

            sb.ToUInt32().Should().Be(0x20100804);
        }

        [Fact]
        public void FromSevenBitToUInt32()
        {
            uint value = 0x1000;
            var sb = SevenBitUInt32.FromSevenBitValue(value);

            sb.ToUInt32().Should().Be(value);
        }
    }
}
