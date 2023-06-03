using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Core.UnitTests
{
    [TestClass]
    public class ValueParserTests
    {
        [TestMethod]
        public void TryParseBytes_4Bytes_Decimal()
        {
            byte[] bytes;

            var success = ValueParser.TryParseToBytes("01 02 03 04", Ordering.BigEndian, out bytes);

            Assert.IsTrue(success);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual(1, bytes[0]);
            Assert.AreEqual(2, bytes[1]);
            Assert.AreEqual(3, bytes[2]);
            Assert.AreEqual(4, bytes[3]);
        }

        [TestMethod]
        public void TryParseBytes_4Bytes_Hexadecimal()
        {
            byte[] bytes;

            var success = ValueParser.TryParseToBytes("B1-C2-D3-E4H", Ordering.BigEndian, out bytes);

            Assert.IsTrue(success);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(4, bytes.Length);
            Assert.AreEqual(0xB1, bytes[0]);
            Assert.AreEqual(0xC2, bytes[1]);
            Assert.AreEqual(0xD3, bytes[2]);
            Assert.AreEqual(0xE4, bytes[3]);
        }

        [TestMethod]
        public void TryParseInt32_4Bytes_Decimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("01-02-03-04", out value);

            Assert.IsTrue(success);
            Assert.AreEqual((int)01020304, value);
        }

        [TestMethod]
        public void TryParseInt32_4Bytes_Hexadecimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("B1-C2-D3-E4H", out value);

            Assert.IsTrue(success);
            Assert.AreEqual((uint)0xB1C2D3E4, (uint)value);
        }

        [TestMethod]
        public void TryParseInt32_OneInteger_Decimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("01020304", out value);

            Assert.IsTrue(success);
            Assert.AreEqual((int)01020304, value);
        }

        [TestMethod]
        public void TryParseInt32_OneInteger_Hexadecimal()
        {
            int value;
            var success = ValueParser.TryParseInt32("B1C2D3E4H", out value);

            Assert.IsTrue(success);
            Assert.AreEqual((uint)0xB1C2D3E4, (uint)value);
        }
    }
}
