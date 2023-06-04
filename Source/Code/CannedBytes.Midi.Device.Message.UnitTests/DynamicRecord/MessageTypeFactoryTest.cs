using System;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Xunit;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    /// <summary>
    ///This is a test class for CannedBytes.Midi.Device.Message.MessageTypeFactory and is intended
    ///to contain all CannedBytes.Midi.Device.Message.MessageTypeFactory Unit Tests
    ///</summary>
    
    //[DeploymentItem("DynamicRecord/TestAddressMap1.mds")]
    //[DeploymentItem("DynamicRecord/TestAddressMap2.mds")]
    public class MessageTypeFactoryTest
    {
        public static readonly string TestAddressMap1FileName = "TestAddressMap1.mds";
        public static readonly string TestAddressMap2FileName = "TestAddressMap2.mds";

        public const string MidiSysExData = "http://schemas.cannedbytes.com/midi-device-schema/midi-types/10:midiData";

        [Fact]
        public void CreateDynamicHierarchicalConverter_2_2_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap1FileName, "MessageOne");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(2));

            Console.WriteLine(converter.ToString());

            Assert.NotNull(converter);
            Assert.Equal(2, converter.RecordType.Fields.Count);
            Assert.Equal("SectionOne", converter.RecordType.Fields[0].Name.Name);
            Assert.Equal("SectionOne", converter.RecordType.Fields[1].Name.Name);

            Assert.Equal(1, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.Equal("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.Equal(1, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.Equal("Field1One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);

            Assert.Equal(2, converter.FieldConverterMap.Count);
            Assert.Equal("SectionOne", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.Equal("SectionOne", converter.FieldConverterMap[1].Field.Name.Name);
        }

        [Fact]
        public void CreateDynamicHierarchicalConverter_2_6_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap1FileName, "MessageOne");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(6));

            Console.WriteLine(converter.ToString());

            Assert.NotNull(converter);
            Assert.Equal(3, converter.RecordType.Fields.Count);
            Assert.Equal("SectionOne", converter.RecordType.Fields[0].Name.Name);
            Assert.Equal("SectionOne", converter.RecordType.Fields[1].Name.Name);
            Assert.Equal("SectionTwo", converter.RecordType.Fields[2].Name.Name);

            Assert.Equal(1, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.Equal("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.Equal(3, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.Equal("Field1One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);
            Assert.Equal("Field1Two", converter.RecordType.Fields[1].RecordType.Fields[1].Name.Name);
            Assert.Equal("Field1Three", converter.RecordType.Fields[1].RecordType.Fields[2].Name.Name);

            Assert.Equal(2, converter.RecordType.Fields[2].RecordType.Fields.Count);
            Assert.Equal("Field2One", converter.RecordType.Fields[2].RecordType.Fields[0].Name.Name);
            Assert.Equal("Field2Two", converter.RecordType.Fields[2].RecordType.Fields[1].Name.Name);

            Assert.Equal(3, converter.FieldConverterMap.Count);
            Assert.Equal("SectionOne", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.Equal("SectionOne", converter.FieldConverterMap[1].Field.Name.Name);
            Assert.Equal("SectionTwo", converter.FieldConverterMap[2].Field.Name.Name);
        }

        [Fact]
        public void CreateDynamicHierarchicalConverter_Hierarchy_2_4_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap2FileName, "HierarchicalDynamicRecordTest");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(4));

            Console.WriteLine(converter.ToString());

            Assert.NotNull(converter);
            // AddressMap/Section
            // AddressMap/Record
            Assert.Equal(2, converter.RecordType.Fields.Count);
            Assert.Equal("Section", converter.RecordType.Fields[0].Name.Name);
            Assert.Equal("Record", converter.RecordType.Fields[1].Name.Name);

            // AddressMap/Section/NestedRecord
            // AddressMap/Section/Field
            Assert.Equal(2, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.Equal("NestedRecord", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.Equal("Field", converter.RecordType.Fields[0].RecordType.Fields[1].Name.Name);

            // AddressMap/Section/NestedRecord/Field1Three
            Assert.Equal(1, converter.RecordType.Fields[0].RecordType.Fields[0].RecordType.Fields.Count);
            Assert.Equal("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].RecordType.Fields[0].Name.Name);

            // AddressMap/Record/Field2One
            // AddressMap/Record/Field2Two
            Assert.Equal(2, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.Equal("Field2One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);
            Assert.Equal("Field2Two", converter.RecordType.Fields[1].RecordType.Fields[1].Name.Name);

            Assert.Equal(2, converter.FieldConverterMap.Count);
            Assert.Equal("Section", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.Equal("Record", converter.FieldConverterMap[1].Field.Name.Name);
        }

        // Helper to create a test MessageTypeFactory instance.
        private MessageTypeFactory CreateMessageTypeFactory(string schemaFileName, string messageName)
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(schemaFileName);
            RecordType message = schema.RootRecordTypes.Find(messageName);

            var container = DeviceHelper.CreateContainer();
            ConverterManager converterManager = new ConverterManager();
            converterManager.InitializeFrom(container);

            GroupConverter baseConverter = converterManager.GetConverter(message);

            Field addressMapField = message.Fields.Find("AddressMap");
            FieldConverterPair addressMapPair = baseConverter.FieldConverterMap.Find(addressMapField);

            MidiDeviceBinaryMap binaryMap = new MidiDeviceBinaryMap(baseConverter);
            Console.WriteLine(binaryMap.ToString());

            var schemaProvider = container.GetExportedValue<IDeviceSchemaProvider>();
            MessageTypeFactory factory = new MessageTypeFactory(converterManager, binaryMap, schemaProvider);

            return factory;
        }
    }
}