using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    [TestClass]
    public class SevenBitUInt32Tests
    {
        [TestMethod]
        public void Construct1Byte_ToInt32_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x00, 0x00, 0x00, 0x10);
            var actual = sbInt.ToInt32();

            Assert.AreEqual(0x10, actual);
        }

        [TestMethod]
        public void Construct2Bytes_ToInt32_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x00, 0x00, 0x10, 0x10);
            var actual = sbInt.ToInt32();

            Assert.AreEqual(0x1010, actual);
        }

        [TestMethod]
        public void Construct3Bytes_ToInt32_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x00, 0x10, 0x10, 0x10);
            var actual = sbInt.ToInt32();

            Assert.AreEqual(0x101010, actual);
        }

        [TestMethod]
        public void Construct4Bytes_ToInt32_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x10, 0x10, 0x10, 0x10);
            var actual = sbInt.ToInt32();

            Assert.AreEqual(0x10101010, actual);
        }

        [TestMethod]
        public void ConstructInt1_ToBytes_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x10);

            Assert.AreEqual(0x10, sbInt.Byte0);
            Assert.AreEqual(0x00, sbInt.Byte1);
            Assert.AreEqual(0x00, sbInt.Byte2);
            Assert.AreEqual(0x00, sbInt.Byte3);
        }

        [TestMethod]
        public void ConstructInt2_ToBytes_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x1010);

            Assert.AreEqual(0x10, sbInt.Byte0);
            Assert.AreEqual(0x10, sbInt.Byte1);
            Assert.AreEqual(0x00, sbInt.Byte2);
            Assert.AreEqual(0x00, sbInt.Byte3);
        }

        [TestMethod]
        public void ConstructInt3_ToBytes_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x101010);

            Assert.AreEqual(0x10, sbInt.Byte0);
            Assert.AreEqual(0x10, sbInt.Byte1);
            Assert.AreEqual(0x10, sbInt.Byte2);
            Assert.AreEqual(0x00, sbInt.Byte3);
        }

        [TestMethod]
        public void ConstructInt4_ToBytes_CorrectConversion()
        {
            var sbInt = new SevenBitUInt32(0x10101010);

            Assert.AreEqual(0x10, sbInt.Byte0);
            Assert.AreEqual(0x10, sbInt.Byte1);
            Assert.AreEqual(0x10, sbInt.Byte2);
            Assert.AreEqual(0x10, sbInt.Byte3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructBytes_ValueTooLarge_Exception()
        {
            var sbInt = new SevenBitUInt32(0x00, 0x00, 0x80, 0x00);
        }

        [TestMethod]
        public void ConstructInt_ValueTooLarge_CorrectValue()
        {
            var sbInt = new SevenBitUInt32(0x00008000);

            Assert.AreEqual(0x00010000, sbInt);
        }

        [TestMethod]
        public void Add_NoOverflow_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x10);
            var actual = sbInt + 0x10;

            Assert.AreEqual(0x20, actual);
        }

        [TestMethod]
        public void Add_OverflowByte1_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x7F);
            var actual = sbInt + 0x01;

            Assert.AreEqual(0x100, actual);
        }

        [TestMethod]
        public void Add_OverflowByte2_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x7F7F);
            var actual = sbInt + 0x01;

            Assert.AreEqual(0x10000, actual);
        }

        [TestMethod]
        public void Add_OverflowByte3_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x7F7F7F);
            var actual = sbInt + 0x01;

            Assert.AreEqual(0x1000000, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Add_OverflowByte4_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x7F7F7F7F);
            var actual = sbInt + 0x01;

            // complete overflow
            Assert.Fail();
        }

        [TestMethod]
        public void Subtract_NoUnderflow_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x0A);
            var actual = sbInt - 5;

            Assert.AreEqual(0x05, actual);
        }

        [TestMethod]
        public void Subtract_UnderflowByte1_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x0100);
            var actual = sbInt - 0x01;

            Assert.AreEqual(0x7F, actual);
        }

        [TestMethod]
        public void Subtract_UnderflowByte2_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x010000);
            var actual = sbInt - 0x01;

            Assert.AreEqual(0x7F7F, actual);
        }

        [TestMethod]
        public void Subtract_UnderflowByte3_CorrectAnswer()
        {
            var sbInt = new SevenBitUInt32(0x01000000);
            var actual = sbInt - 0x01;

            Assert.AreEqual(0x7F7F7F, actual);
        }
    }
}