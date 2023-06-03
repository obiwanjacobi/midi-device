using System.IO;
using CannedBytes.Midi.Device.Schema.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests.Xml
{
    [TestClass]
    [DeploymentItem("Xml/DeviceSchema1.mds")]
    [DeploymentItem("Xml/DeviceSchema2.mds")]
    [DeploymentItem("Xml/DeviceSchema3.mds")]
    [DeploymentItem("Xml/InvalidDataTypeBase.mds")]
    [DeploymentItem("Xml/InvalidRecordTypeBase.mds")]
    [DeploymentItem("Xml/InvalidFieldType.mds")]
    public class ParserTest
    {
        [TestMethod]
        public void Parse_Schema1_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema1.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.IsNotNull(schema);
                Assert.AreNotEqual(0, schema.AllDataTypes.Count);
                Assert.AreNotEqual(0, schema.AllRecordTypes.Count);

                Assert.IsNotNull(schema.AllDataTypes.Find("midiByte"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiData"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiNull"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiLSNibble"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiChannel"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiStatus"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiMSNibble"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiNibble"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit0"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit1"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit2"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit3"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit4"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit5"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit6"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit7"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit8"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit9"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit10"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit11"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit12"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit13"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit14"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiBit15"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiComposite"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiString"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiSigned"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned16"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned24"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned32"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned40"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned48"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned56"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiUnsigned64"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiSigned16"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiSigned32"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiSigned64"));
                Assert.IsNotNull(schema.AllDataTypes.Find("midiChecksum"));

                Assert.IsNotNull(schema.AllRecordTypes.Find("midiSplitNibbleBE"));
                Assert.IsNotNull(schema.AllRecordTypes.Find("midiSplitNibbleLE"));
                Assert.IsNotNull(schema.AllRecordTypes.Find("midiBigEndian"));
                Assert.IsNotNull(schema.AllRecordTypes.Find("midiLittleEndian"));
            }
        }

        [TestMethod]
        public void Parse_Import_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema2.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.IsNotNull(schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/XmlUnitTests/DeviceSchema1"));

                Assert.IsNotNull(schema);
                Assert.AreNotEqual(0, schema.AllDataTypes.Count);

                Assert.IsNotNull(schema.AllDataTypes.Find("derivedDataType"));
            }
        }

        [TestMethod]
        public void Parse_ImportResource_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema3.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.IsNotNull(schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/midi-types/10"));

                Assert.IsNotNull(schema);
                Assert.AreNotEqual(0, schema.AllDataTypes.Count);

                var dataType = schema.AllDataTypes.Find("derivedDataType");
                Assert.IsNotNull(dataType);

                var recordType = schema.AllRecordTypes.Find("testRecord");
                Assert.IsNotNull(recordType);

                Assert.AreEqual(1, recordType.Fields.Count);
                var field = recordType.Fields[0];
                Assert.IsNotNull(field);

                Assert.AreEqual("Field1", field.Name.Name);
                Assert.AreEqual(dataType, field.DataType);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceSchemaException))]
        public void Parse_InvalidDataType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidDataTypeBase.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceSchemaException))]
        public void Parse_InvalidRecordType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidRecordTypeBase.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceSchemaException))]
        public void Parse_InvalidFieldType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidFieldType.mds"))
            {
                var schema = parser.Parse(stream);

                Assert.Fail();
            }
        }
    }
}