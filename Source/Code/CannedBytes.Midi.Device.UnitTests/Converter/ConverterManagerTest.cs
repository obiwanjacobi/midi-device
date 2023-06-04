using System.ComponentModel.Composition.Hosting;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.Converter
{
    
    public class ConverterManagerTest
    {
        private static DeviceSchema MidiTypesSchema = DeviceHelper.OpenDeviceSchema("CannedBytes.Midi.Device/MidiTypes.mds");
        private static CompositionContainer container = DeviceHelper.CreateContainer();

        private void TestManagerDataType(string dataTypeName, System.Type converterType)
        {
            var converterMgr = new ConverterManager();
            converterMgr.InitializeFrom(container);

            var dataType = MidiTypesSchema.AllDataTypes.Find(dataTypeName);

            Assert.NotNull(dataType);

            var converter = converterMgr.GetConverter(dataType);

            if (converterType == null)
            {
                Assert.Null(converter);
            }
            else
            {
                Assert.IsType(converterType, converter);
            }
        }

        private void TestManagerRecordType(string recordTypeName, System.Type converterType)
        {
            var converterMgr = new ConverterManager();
            converterMgr.InitializeFrom(container);

            var recordType = MidiTypesSchema.AllRecordTypes.Find(recordTypeName);

            Assert.NotNull(recordType);

            var converter = converterMgr.GetConverter(recordType);

            if (converterType == null)
            {
                Assert.Null(converter);
            }
            else
            {
                Assert.IsType(converterType, converter);
            }
        }

        [Fact]
        public void ManagerGetConverter_MidiBit0_BitConverter()
        {
            TestManagerDataType("midiBit0", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit1_BitConverter()
        {
            TestManagerDataType("midiBit1", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit2_BitConverter()
        {
            TestManagerDataType("midiBit2", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit3_BitConverter()
        {
            TestManagerDataType("midiBit3", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit4_BitConverter()
        {
            TestManagerDataType("midiBit4", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit5_BitConverter()
        {
            TestManagerDataType("midiBit5", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit6_BitConverter()
        {
            TestManagerDataType("midiBit6", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBit7_BitConverter()
        {
            TestManagerDataType("midiBit7", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiByte_BitConverter()
        {
            TestManagerDataType("midiByte", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiChannel_BitConverter()
        {
            TestManagerDataType("midiChannel", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiChecksum_ChecksumConverter()
        {
            TestManagerDataType("midiChecksum", typeof(ChecksumConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiComposite_Null()
        {
            TestManagerDataType("midiComposite", null);
        }

        [Fact]
        public void ManagerGetConverter_MidiLSNibble_BitConverter()
        {
            TestManagerDataType("midiLSNibble", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiMSNibble_BitConverter()
        {
            TestManagerDataType("midiMSNibble", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiNibble_BitConverter()
        {
            TestManagerDataType("midiNibble", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiNull_NullByteConverter()
        {
            TestManagerDataType("midiNull", typeof(NullByteConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSigned_SignedConverter()
        {
            TestManagerDataType("midiSigned", typeof(SignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSigned16_SignedConverter()
        {
            TestManagerDataType("midiSigned16", typeof(SignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSigned32_SignedConverter()
        {
            TestManagerDataType("midiSigned32", typeof(SignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSigned64_SignedConverter()
        {
            TestManagerDataType("midiSigned64", typeof(SignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiStatus_BitConverter()
        {
            TestManagerDataType("midiStatus", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiString_StringConverter()
        {
            System.Action action = () => TestManagerDataType("midiString", typeof(StringConverter));
            action.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void ManagerGetConverter_MidiSysExData_BitConverter()
        {
            TestManagerDataType("midiData", typeof(BitConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned16", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned16_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned16", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned24_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned24", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned32_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned32", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned40_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned40", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned48_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned48", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned56_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned56", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiUnsigned64_UnsignedConverter()
        {
            TestManagerDataType("midiUnsigned64", typeof(UnsignedConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiBigEndian_GroupConverter()
        {
            TestManagerRecordType("midiBigEndian", typeof(BigEndianGroupConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiLittleEndian_GroupConverter()
        {
            TestManagerRecordType("midiLittleEndian", typeof(GroupConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSplitNibbleLE_SplitNibbleLEGroupConverter()
        {
            TestManagerRecordType("midiSplitNibbleLE", typeof(SplitNibbleLEGroupConverter));
        }

        [Fact]
        public void ManagerGetConverter_MidiSplitNibbleBE_SplitNibbleBEGroupConverter()
        {
            TestManagerRecordType("midiSplitNibbleBE", typeof(SplitNibbleBEGroupConverter));
        }
    }
}