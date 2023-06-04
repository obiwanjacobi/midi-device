using System;
using System.IO;
using CannedBytes.Midi.Device.Schema.Xml;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests.Xml
{
    
    //[DeploymentItem("Xml/DeviceSchema1.mds")]
    //[DeploymentItem("Xml/DeviceSchema2.mds")]
    //[DeploymentItem("Xml/DeviceSchema3.mds")]
    //[DeploymentItem("Xml/InvalidDataTypeBase.mds")]
    //[DeploymentItem("Xml/InvalidRecordTypeBase.mds")]
    //[DeploymentItem("Xml/InvalidFieldType.mds")]
    public class ParserTest
    {
        [Fact]
        public void Parse_Schema1_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema1.mds"))
            {
                var schema = parser.Parse(stream);
                
                schema.Should().NotBeNull();
                schema.AllDataTypes.Should().NotBeEmpty();
                schema.AllRecordTypes.Should().NotBeEmpty();

                var dataTypes = new[] {
                    "midiByte",
                    "midiData",
                    "midiNull",
                    "midiLSNibble",
                    "midiChannel",
                    "midiStatus",
                    "midiMSNibble",
                    "midiNibble",
                    "midiBit0",
                    "midiBit1",
                    "midiBit2",
                    "midiBit3",
                    "midiBit4",
                    "midiBit5",
                    "midiBit6",
                    "midiBit7",
                    "midiBit8",
                    "midiBit9",
                    "midiBit10",
                    "midiBit11",
                    "midiBit12",
                    "midiBit13",
                    "midiBit14",
                    "midiBit15",
                    "midiComposite",
                    "midiString",
                    "midiUnsigned",
                    "midiSigned",
                    "midiUnsigned16",
                    "midiUnsigned24",
                    "midiUnsigned32",
                    "midiUnsigned40",
                    "midiUnsigned48",
                    "midiUnsigned56",
                    "midiUnsigned64",
                    "midiSigned16",
                    "midiSigned32",
                    "midiSigned64",
                    "midiChecksum"
                };

                foreach (var dt in dataTypes)
                {
                    schema.AllDataTypes.Find(dt).Should().NotBeNull();
                }

                var recordTypes = new[]
                {
                    "midiSplitNibbleBE",
                    "midiSplitNibbleLE",
                    "midiBigEndian",
                    "midiLittleEndian"
                };

                foreach (var rt in recordTypes)
                {
                    schema.AllRecordTypes.Find(rt).Should().NotBeNull();
                }
            }
        }

        [Fact]
        public void Parse_Import_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema2.mds"))
            {
                var schema = parser.Parse(stream);

                schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/XmlUnitTests/DeviceSchema1").Should().NotBeNull();

                schema.Should().NotBeNull();
                schema.AllDataTypes.Should().NotBeEmpty();
                schema.AllDataTypes.Find("derivedDataType").Should().NotBeNull();
            }
        }

        [Fact]
        public void Parse_ImportResource_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema3.mds"))
            {
                var schema = parser.Parse(stream);

                schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/midi-types/10").Should().NotBeNull();

                schema.Should().NotBeNull();
                schema.AllDataTypes.Should().NotBeEmpty();

                var dataType = schema.AllDataTypes.Find("derivedDataType");
                dataType.Should().NotBeNull();

                var recordType = schema.AllRecordTypes.Find("testRecord");
                recordType.Should().NotBeNull();

                recordType.Fields.Should().HaveCount(1);
                var field = recordType.Fields[0];
                field.Should().NotBeNull();

                field.Name.Name.Should().Be("Field1");
                field.DataType.Should().Be(dataType);
            }
        }

        [Fact]
        public void Parse_InvalidDataType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidDataTypeBase.mds"))
            {
                Action action = () => parser.Parse(stream);
                action.Should().Throw<DeviceSchemaException>();
            }
        }

        [Fact]
        public void Parse_InvalidRecordType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidRecordTypeBase.mds"))
            {
                Action action = () => parser.Parse(stream);
                action.Should().Throw<DeviceSchemaException>();
            }
        }

        [Fact]
        public void Parse_InvalidFieldType_Exception()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("InvalidFieldType.mds"))
            {
                Action action = () => parser.Parse(stream);
                action.Should().Throw<DeviceSchemaException>();
            }
        }
    }
}