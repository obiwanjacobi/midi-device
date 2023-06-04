using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.UnitTests.Converter
{
    
    public class ConverterFactoryTest
    {
        private static DeviceSchema MidiTypesSchema = DeviceHelper.OpenDeviceSchema("CannedBytes.Midi.Device/MidiTypes.mds");
        private static MidiTypesConverterFactory ConverterFactory = new MidiTypesConverterFactory();

        private void TestFactoryDataType(string dataTypeName, System.Type converterType)
        {
            var dataType = MidiTypesSchema.AllDataTypes.Find(dataTypeName);

            Assert.NotNull(dataType);

            var converter = ConverterFactory.Create(dataType, dataType);

            if (converterType == null)
            {
                Assert.Null(converter);
            }
            else
            {
                Assert.IsType(converterType, converter);
            }
        }

        private void TestFactoryRecordType(string recordTypeName, System.Type converterType)
        {
            var recordType = MidiTypesSchema.AllRecordTypes.Find(recordTypeName);

            Assert.NotNull(recordType);

            var converter = ConverterFactory.Create(recordType, recordType);

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
        public void FactoryCreate_MidiBit0_BitConverter()
        {
            TestFactoryDataType("midiBit0", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit1_BitConverter()
        {
            TestFactoryDataType("midiBit1", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit2_BitConverter()
        {
            TestFactoryDataType("midiBit2", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit3_BitConverter()
        {
            TestFactoryDataType("midiBit3", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit4_BitConverter()
        {
            TestFactoryDataType("midiBit4", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit5_BitConverter()
        {
            TestFactoryDataType("midiBit5", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit6_BitConverter()
        {
            TestFactoryDataType("midiBit6", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiBit7_BitConverter()
        {
            TestFactoryDataType("midiBit7", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiByte_BitConverter()
        {
            TestFactoryDataType("midiByte", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiChannel_Null()
        {
            TestFactoryDataType("midiChannel", null);
        }

        [Fact]
        public void FactoryCreate_MidiChecksum_ChecksumConverter()
        {
            TestFactoryDataType("midiChecksum", typeof(ChecksumConverter));
        }

        [Fact]
        public void FactoryCreate_MidiComposite_Null()
        {
            TestFactoryDataType("midiComposite", null);
        }

        [Fact]
        public void FactoryCreate_MidiLSNibble_BitConverter()
        {
            TestFactoryDataType("midiLSNibble", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiMSNibble_BitConverter()
        {
            TestFactoryDataType("midiMSNibble", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiNibble_BitConverter()
        {
            TestFactoryDataType("midiNibble", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiNull_NullByteConverter()
        {
            TestFactoryDataType("midiNull", typeof(NullByteConverter));
        }

        [Fact]
        public void FactoryCreate_MidiSigned_SignedConverter()
        {
            TestFactoryDataType("midiSigned", typeof(SignedConverter));
        }

        [Fact]
        public void FactoryCreate_MidiSigned16_Null()
        {
            TestFactoryDataType("midiSigned16", null);
        }

        [Fact]
        public void FactoryCreate_MidiSigned32_Null()
        {
            TestFactoryDataType("midiSigned32", null);
        }

        [Fact]
        public void FactoryCreate_MidiSigned64_Null()
        {
            TestFactoryDataType("midiSigned64", null);
        }

        [Fact]
        public void FactoryCreate_MidiStatus_Null()
        {
            TestFactoryDataType("midiStatus", null);
        }

        [Fact]
        public void FactoryCreate_MidiString_StringConverter()
        {
            System.Action action = () => TestFactoryDataType("midiString", typeof(StringConverter));
            action.Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void FactoryCreate_MidiSysExData_BitConverter()
        {
            TestFactoryDataType("midiData", typeof(BitConverter));
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned_UnsignedConverter()
        {
            TestFactoryDataType("midiUnsigned", typeof(UnsignedConverter));
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned16_Null()
        {
            TestFactoryDataType("midiUnsigned16", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned24_Null()
        {
            TestFactoryDataType("midiUnsigned24", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned32_Null()
        {
            TestFactoryDataType("midiUnsigned32", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned40_Null()
        {
            TestFactoryDataType("midiUnsigned40", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned48_Null()
        {
            TestFactoryDataType("midiUnsigned48", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned56_Null()
        {
            TestFactoryDataType("midiUnsigned56", null);
        }

        [Fact]
        public void FactoryCreate_MidiUnsigned64_Null()
        {
            TestFactoryDataType("midiUnsigned64", null);
        }

        [Fact]
        public void FactoryCreate_MidiBigEndian_GroupConverter()
        {
            TestFactoryRecordType("midiBigEndian", typeof(BigEndianGroupConverter));
        }

        [Fact]
        public void FactoryCreate_MidiLittleEndian_GroupConverter()
        {
            TestFactoryRecordType("midiLittleEndian", typeof(GroupConverter));
        }

        [Fact]
        public void FactoryCreate_MidiSplitNibbleLE_SplitNibbleLEGroupConverter()
        {
            TestFactoryRecordType("midiSplitNibbleLE", typeof(SplitNibbleLEGroupConverter));
        }

        [Fact]
        public void FactoryCreate_MidiSplitNibbleBE_SplitNibbleBEGroupConverter()
        {
            TestFactoryRecordType("midiSplitNibbleBE", typeof(SplitNibbleBEGroupConverter));
        }
    }
}