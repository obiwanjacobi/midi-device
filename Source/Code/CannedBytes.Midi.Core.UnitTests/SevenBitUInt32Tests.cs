using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Core.UnitTests
{
    [TestClass]
    public class SevenBitUInt32Tests
    {
        [TestMethod]
        public void Ctor_ParseHex()
        {
            var sb = new SevenBitUInt32("20-10-08-04H");

            Assert.AreEqual((uint)0x20100804, sb.ToUInt32());
        }

        [TestMethod]
        public void FromSevenBitToUInt32()
        {
            uint value = 0x1000;

            var sb = SevenBitUInt32.FromSevenBitValue(value);

            Assert.AreEqual(value, sb.ToUInt32());
        }
    }
}
