using System;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Core.UnitTests
{
    
    public class ValueParserTests
    {
        [Fact]
        public void TryParseBytes_4Bytes_Decimal()
        {
            byte[] bytes;

            var success = ValueParser.TryParseToBytes("01 02 03 04", Ordering.BigEndian, out bytes);

            success.Should().BeTrue();
            bytes.Should().NotBeNull();
            bytes.Should().HaveCount(4);
            bytes[0].Should().Be(1);
            bytes[1].Should().Be(2);
            bytes[2].Should().Be(3);
            bytes[3].Should().Be(4);
        }

        [Fact]
        public void TryParseBytes_4Bytes_Hexadecimal()
        {
            byte[] bytes;

            var success = ValueParser.TryParseToBytes("B1-C2-D3-E4H", Ordering.BigEndian, out bytes);

            success.Should().BeTrue();
            bytes.Should().NotBeNull();
            bytes.Should().HaveCount(4);
            bytes[0].Should().Be(0xB1);
            bytes[1].Should().Be(0xC2);
            bytes[2].Should().Be(0xD3);
            bytes[3].Should().Be(0xE4);
        }

        [Fact]
        public void TryParseInt32_4Bytes_Decimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("01-02-03-04", out value);

            success.Should().BeTrue();
            value.Should().Be(01020304);
        }

        [Fact]
        public void TryParseInt32_4Bytes_Hexadecimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("B1-C2-D3-E4H", out value);

            success.Should().BeTrue();
            value.Should().Be(unchecked((int)0xB1C2D3E4));
        }

        [Fact]
        public void TryParseInt32_OneInteger_Decimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("01020304", out value);

            success.Should().BeTrue();
            value.Should().Be(01020304);
        }

        [Fact]
        public void TryParseInt32_OneInteger_Hexadecimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("B1C2D3E4H", out value);

            success.Should().BeTrue();
            value.Should().Be(unchecked((int)0xB1C2D3E4));
        }
    }
}
