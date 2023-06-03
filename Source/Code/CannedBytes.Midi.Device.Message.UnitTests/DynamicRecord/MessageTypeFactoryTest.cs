using System;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    /// <summary>
    ///This is a test class for CannedBytes.Midi.Device.Message.MessageTypeFactory and is intended
    ///to contain all CannedBytes.Midi.Device.Message.MessageTypeFactory Unit Tests
    ///</summary>
    [TestClass()]
    [DeploymentItem("DynamicRecord/TestAddressMap1.mds")]
    [DeploymentItem("DynamicRecord/TestAddressMap2.mds")]
    public class MessageTypeFactoryTest
    {
        public static readonly string TestAddressMap1FileName = "TestAddressMap1.mds";
        public static readonly string TestAddressMap2FileName = "TestAddressMap2.mds";

        public const string MidiSysExData = "http://schemas.cannedbytes.com/midi-device-schema/midi-types/10:midiData";

        [TestMethod]
        public void CreateDynamicHierarchicalConverter_2_2_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap1FileName, "MessageOne");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(2));

            Console.WriteLine(converter.ToString());

            Assert.IsNotNull(converter);
            Assert.AreEqual(2, converter.RecordType.Fields.Count);
            Assert.AreEqual("SectionOne", converter.RecordType.Fields[0].Name.Name);
            Assert.AreEqual("SectionOne", converter.RecordType.Fields[1].Name.Name);

            Assert.AreEqual(1, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.AreEqual("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.AreEqual(1, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.AreEqual("Field1One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);

            Assert.AreEqual(2, converter.FieldConverterMap.Count);
            Assert.AreEqual("SectionOne", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.AreEqual("SectionOne", converter.FieldConverterMap[1].Field.Name.Name);
        }

        [TestMethod]
        public void CreateDynamicHierarchicalConverter_2_6_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap1FileName, "MessageOne");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(6));

            Console.WriteLine(converter.ToString());

            Assert.IsNotNull(converter);
            Assert.AreEqual(3, converter.RecordType.Fields.Count);
            Assert.AreEqual("SectionOne", converter.RecordType.Fields[0].Name.Name);
            Assert.AreEqual("SectionOne", converter.RecordType.Fields[1].Name.Name);
            Assert.AreEqual("SectionTwo", converter.RecordType.Fields[2].Name.Name);

            Assert.AreEqual(1, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.AreEqual("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.AreEqual(3, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.AreEqual("Field1One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);
            Assert.AreEqual("Field1Two", converter.RecordType.Fields[1].RecordType.Fields[1].Name.Name);
            Assert.AreEqual("Field1Three", converter.RecordType.Fields[1].RecordType.Fields[2].Name.Name);

            Assert.AreEqual(2, converter.RecordType.Fields[2].RecordType.Fields.Count);
            Assert.AreEqual("Field2One", converter.RecordType.Fields[2].RecordType.Fields[0].Name.Name);
            Assert.AreEqual("Field2Two", converter.RecordType.Fields[2].RecordType.Fields[1].Name.Name);

            Assert.AreEqual(3, converter.FieldConverterMap.Count);
            Assert.AreEqual("SectionOne", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.AreEqual("SectionOne", converter.FieldConverterMap[1].Field.Name.Name);
            Assert.AreEqual("SectionTwo", converter.FieldConverterMap[2].Field.Name.Name);
        }

        [TestMethod]
        public void CreateDynamicHierarchicalConverter_Hierarchy_2_4_Test()
        {
            MessageTypeFactory factory = CreateMessageTypeFactory(TestAddressMap2FileName, "HierarchicalDynamicRecordTest");
            GroupConverter converter = factory.CreateDynamicGroupConverter(SevenBitUInt32.FromInt32(2), SevenBitUInt32.FromInt32(4));

            Console.WriteLine(converter.ToString());

            Assert.IsNotNull(converter);
            // AddressMap/Section
            // AddressMap/Record
            Assert.AreEqual(2, converter.RecordType.Fields.Count);
            Assert.AreEqual("Section", converter.RecordType.Fields[0].Name.Name);
            Assert.AreEqual("Record", converter.RecordType.Fields[1].Name.Name);

            // AddressMap/Section/NestedRecord
            // AddressMap/Section/Field
            Assert.AreEqual(2, converter.RecordType.Fields[0].RecordType.Fields.Count);
            Assert.AreEqual("NestedRecord", converter.RecordType.Fields[0].RecordType.Fields[0].Name.Name);
            Assert.AreEqual("Field", converter.RecordType.Fields[0].RecordType.Fields[1].Name.Name);

            // AddressMap/Section/NestedRecord/Field1Three
            Assert.AreEqual(1, converter.RecordType.Fields[0].RecordType.Fields[0].RecordType.Fields.Count);
            Assert.AreEqual("Field1Three", converter.RecordType.Fields[0].RecordType.Fields[0].RecordType.Fields[0].Name.Name);

            // AddressMap/Record/Field2One
            // AddressMap/Record/Field2Two
            Assert.AreEqual(2, converter.RecordType.Fields[1].RecordType.Fields.Count);
            Assert.AreEqual("Field2One", converter.RecordType.Fields[1].RecordType.Fields[0].Name.Name);
            Assert.AreEqual("Field2Two", converter.RecordType.Fields[1].RecordType.Fields[1].Name.Name);

            Assert.AreEqual(2, converter.FieldConverterMap.Count);
            Assert.AreEqual("Section", converter.FieldConverterMap[0].Field.Name.Name);
            Assert.AreEqual("Record", converter.FieldConverterMap[1].Field.Name.Name);
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