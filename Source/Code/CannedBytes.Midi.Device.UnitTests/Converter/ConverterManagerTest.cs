using System.ComponentModel.Composition.Hosting;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.Converter
{
    [TestClass]
    public class ConverterManagerTest
    {
        private static DeviceSchema MidiTypesSchema = DeviceHelper.OpenDeviceSchema("CannedBytes.Midi.Device/MidiTypes.mds");
        private static CompositionContainer container = DeviceHelper.CreateContainer();

        private void TestManagerDataType(string dataTypeName, System.Type converterType)
        {
            var converterMgr = new ConverterManager();
            converterMgr.InitializeFrom(container);

            var dataType = MidiTypesSchema.AllDataTypes.Find(dataTypeName);

            Assert.IsNotNull(dataType, "DataType '" + dataTypeName + "' was not found.");

            var converter = converterMgr.GetConverter(dataType);

            if (converterType == null)
            {
                Assert.IsNull(converter);
            }
            else
            {
                Assert.IsInstanceOfType(converter, converterType);
            }
        }

        private void TestManagerRecordType(string recordTypeName, System.Type converterType)
        {
            var converterMgr = new ConverterManager();
            converterMgr.InitializeFrom(container);

            var recordType = MidiTypesSchema.AllRecordTypes.Find(recordTypeName);

            Assert.IsNotNull(recordType);

            var converter = converterMgr.GetConverter(recordType);

            if (converterType == null)
            {
                Assert.IsNull(converter);
            }
            else
            {
                Assert.IsInstanceOfType(converter, converterType);
            }
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit0_BitConverter()
        {
            TestManagerDataType("midiBit0", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit1_BitConverter()
        {
            TestManagerDataType("midiBit1", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit2_BitConverter()
        {
            TestManagerDataType("midiBit2", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit3_BitConverter()
        {
            TestManagerDataType("midiBit3", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit4_BitConverter()
        {
            TestManagerDataType("midiBit4", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit5_BitConverter()
        {
            TestManagerDataType("midiBit5", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit6_BitConverter()
        {
            TestManagerDataType("midiBit6", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBit7_BitConverter()
        {
            TestManagerDataType("midiBit7", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiByte_BitConverter()
        {
            TestManagerDataType("midiByte", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiChannel_BitConverter()
        {
            TestManagerDataType("midiChannel", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiChecksum_ChecksumConverter()
        {
            TestManagerDataType("midiChecksum", typeof(ChecksumConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiComposite_Null()
        {
            TestManagerDataType("midiComposite", null);
        }

        [TestMethod]
        public void ManagerGetConverter_MidiLSNibble_BitConverter()
        {
            TestManagerDataType("midiLSNibble", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiMSNibble_BitConverter()
        {
            TestManagerDataType("midiMSNibble", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiNibble_BitConverter()
        {
            TestManagerDataType("midiNibble", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiNull_NullByteConverter()
        {
            TestManagerDataType("midiNull", typeof(NullByteConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSigned_SignedConverter()
        {
            TestManagerDataType("midiSigned", typeof(SignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSigned16_SignedConverter()
        {
            TestManagerDataType("midiSigned16", typeof(SignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSigned32_SignedConverter()
        {
            TestManagerDataType("midiSigned32", typeof(SignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSigned64_SignedConverter()
        {
            TestManagerDataType("midiSigned64", typeof(SignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiStatus_BitConverter()
        {
            TestManagerDataType("midiStatus", typeof(BitConverter));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void ManagerGetConverter_MidiString_StringConverter()
        {
            TestManagerDataType("midiString", typeof(StringConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSysExData_BitConverter()
        {
            TestManagerDataType("midiData", typeof(BitConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned16", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned16_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned16", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned24_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned24", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned32_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned32", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned40_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned40", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned48_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned48", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned56_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned56", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiUnsigned64_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned64", typeof(UnsignedConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiBigEndian_GroupConverter()
        {
            TestManagerRecordType("midiBigEndian", typeof(BigEndianGroupConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiLittleEndian_GroupConverter()
        {
            TestManagerRecordType("midiLittleEndian", typeof(GroupConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSplitNibbleLE_SplitNibbleLEGroupConverter()
        {
            TestManagerRecordType("midiSplitNibbleLE", typeof(SplitNibbleLEGroupConverter));
        }

        [TestMethod]
        public void ManagerGetConverter_MidiSplitNibbleBE_SplitNibbleBEGroupConverter()
        {
            TestManagerRecordType("midiSplitNibbleBE", typeof(SplitNibbleBEGroupConverter));
        }
    }
}