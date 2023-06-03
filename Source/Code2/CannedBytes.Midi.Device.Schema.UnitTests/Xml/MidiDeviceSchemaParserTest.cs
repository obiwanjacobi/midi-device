using System;
using System.IO;
using System.Linq;
using CannedBytes.Midi.Device.Schema.Xml;
using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests.Xml
{
    
    //[DeploymentItem("Xml/DataType.mds")]
    //[DeploymentItem("Xml/RecordType.mds")]
    //[DeploymentItem("Xml/DataTypeConstraints.mds")]
    //[DeploymentItem("Xml/DeviceSchemaTypes.mds")]
    //[DeploymentItem("Xml/DataTypeFromImport.mds")]
    //[DeploymentItem("Xml/DeviceSchema3.mds")]
    //[DeploymentItem("Xml/InvalidDataTypeBase.mds")]
    //[DeploymentItem("Xml/InvalidRecordTypeBase.mds")]
    //[DeploymentItem("Xml/InvalidFieldType.mds")]
    public class MidiDeviceSchemaParserTest
    {
        public class ParserTestState : IDisposable
        {
            public MidiDeviceSchemaSet Schemas;
            public MidiDeviceSchemaParser Parser;
            public Stream Stream;

            public DeviceSchema Parse()
            {
                return Parser.Parse(Stream);
            }

            public void Dispose()
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }
            }
        }

        public static ParserTestState CreateParserTestState(string schemaPath)
        {
            var state = new ParserTestState();

            state.Schemas = new MidiDeviceSchemaSet();
            state.Parser = new MidiDeviceSchemaParser(state.Schemas);
            state.Stream = File.OpenRead(schemaPath);

            return state;
        }

        public static DeviceSchema ParseSchema(string schemaPath)
        {
            DeviceSchema schema = null;

            using (var state = CreateParserTestState(schemaPath))
            {
                schema = state.Parse();
            }

            return schema;
        }


        // TODO: Move this to CannedBytes.Midi.Device.IntegrationTests
        //[Fact]
        public void Parse_ImportResource_NoErrors()
        {
            var schemas = new MidiDeviceSchemaSet();
            var parser = new MidiDeviceSchemaParser(schemas);

            using (var stream = File.OpenRead("DeviceSchema3.mds"))
            {
                var schema = parser.Parse(stream);

                schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/midi-types/10").Should().NotBeNull();

                schema.Should().NotBeNull();
                schema.AllDataTypes.Should().BeEmpty();

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
        public void Parse_DeviceSchemaTypes_ReturnsInstance()
        {
            var schema = ParseSchema("DeviceSchemaTypes.mds");

            schema.Should().NotBeNull();
        }

        [Fact]
        public void Parse_DeviceSchemaTypes_HasSchemaDocumentation()
        {
            var schema = ParseSchema("DeviceSchemaTypes.mds");

            schema.Attributes.Should().NotBeEmpty();
            schema.Attributes.Find("Documentation").Should().NotBeNull();
        }

        [Fact]
        public void Parse_DeviceSchemaTypes_HasSchemaDocumentationText()
        {
            var schema = ParseSchema("DeviceSchemaTypes.mds");

            var docAttr = schema.Attributes.Find("Documentation");
            docAttr.Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Parse_DataTypeSchema_ReturnsInstance()
        {
            var schema = ParseSchema("DataType.mds");

            schema.Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeSchema_ContainsDataTypes()
        {
            var schema = ParseSchema("DataType.mds");

            schema.AllDataTypes.Should().NotBeEmpty();
        }

        [Fact]
        public void Parse_DataTypeSchema_ContainsRootDataType()
        {
            var schema = ParseSchema("DataType.mds");

            schema.AllDataTypes.Find("midiByte").Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeSchema_ContainsDerivedDataType()
        {
            var schema = ParseSchema("DataType.mds");

            schema.AllDataTypes.Find("midiData").Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeSchema_HasTypeDocumentation()
        {
            var schema = ParseSchema("DataType.mds");
            var type = schema.AllDataTypes.Find("midiByte");
            
            type.Attributes.Should().NotBeNull();
            type.Attributes.Should().NotBeEmpty();
            type.Attributes.Find("Documentation").Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeSchema_HasTypeDocumentationText()
        {
            var schema = ParseSchema("DataType.mds");
            var type = schema.AllDataTypes.Find("midiByte");

            var docAttr = type.Attributes.Find("Documentation");
            docAttr.Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasTypeWithConstraints()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");
            
            type.Should().NotBeNull();
            type.Constraints.Should().NotBeEmpty();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasMinConstraint()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            type.Constraints.Find(ConstraintTypes.MinInclusive).Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_MinConstraintValidates()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            var constraint = type.Constraints.Find(ConstraintTypes.MinInclusive);
            constraint.Validate(0).Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_MinConstraintValidationFails()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            var constraint = type.Constraints.Find(ConstraintTypes.MinInclusive);
            constraint.Validate(-1).Should().BeFalse();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasMaxConstraint()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            type.Constraints.Find(ConstraintTypes.MaxInclusive).Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_MaxConstraintValidates()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            var constraint = type.Constraints.Find(ConstraintTypes.MaxInclusive);
            constraint.Validate(127).Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_MaxConstraintValidationFails()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiData");

            var constraint = type.Constraints.Find(ConstraintTypes.MaxInclusive);
            constraint.Validate(128).Should().BeFalse();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasEnumConstraint()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiStatus");

            type.Constraints.Find(ConstraintTypes.Enumeration).Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_EnumConstraintValidatesFirst()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiStatus");

            var constraint = type.Constraints.FindAll(ConstraintTypes.Enumeration).First();
            constraint.Validate(8).Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_EnumConstraintValidatesLast()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiStatus");

            var constraint = type.Constraints.FindAll(ConstraintTypes.Enumeration).Last();
            constraint.Validate(15).Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_EnumConstraintValidationFails()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiStatus");

            var constraint = type.Constraints.Find(ConstraintTypes.Enumeration);
            constraint.Validate(0).Should().BeFalse();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasLengthConstraint()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiString");

            type.Constraints.Find(ConstraintTypes.FixedLength).Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_LengthConstraintValidates()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiString");

            var constraint = type.Constraints.Find(ConstraintTypes.FixedLength);
            constraint.Validate("0123456789").Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_LengthConstraintValidationFails()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("midiString");

            var constraint = type.Constraints.Find(ConstraintTypes.FixedLength);
            constraint.Validate("01234567890").Should().BeFalse();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_HasFixedConstraint()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("modelId");

            type.Constraints.Find(ConstraintTypes.FixedValue).Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_FixedConstraintValidates()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("modelId");

            var constraint = type.Constraints.Find(ConstraintTypes.FixedValue);
            constraint.Validate(16).Should().BeTrue();
        }

        [Fact]
        public void Parse_DataTypeConstraintSchema_FixedConstraintValidationFails()
        {
            var schema = ParseSchema("DataTypeConstraints.mds");
            var type = schema.AllDataTypes.Find("modelId");

            var constraint = type.Constraints.Find(ConstraintTypes.FixedValue);
            constraint.Validate(0).Should().BeFalse();
        }

        [Fact]
        public void Parse_RecordTypeSchema_ReturnsInstance()
        {
            var schema = ParseSchema("RecordType.mds");

            schema.Should().NotBeNull();
        }

        [Fact]
        public void Parse_RecordTypeSchema_ContainsRecordTypes()
        {
            var schema = ParseSchema("RecordType.mds");

            schema.AllRecordTypes.Should().NotBeEmpty();
        }

        [Fact]
        public void Parse_RecordTypeSchema_ContainsVirtualRootFields()
        {
            var schema = ParseSchema("RecordType.mds");

            schema.VirtualRootFields.Should().HaveCount(schema.RootRecordTypes.Count);
        }

        [Fact]
        public void Parse_RecordTypeSchema_ContainsRootRecordType()
        {
            var schema = ParseSchema("RecordType.mds");

            schema.AllRecordTypes.Find("midiBigEndian").Should().NotBeNull();
        }

        [Fact]
        public void Parse_RecordTypeSchema_ContainsDerivedRecordType()
        {
            var schema = ParseSchema("RecordType.mds");

            schema.AllRecordTypes.Find("midiSplitNibbleBE").Should().NotBeNull();
        }

        [Fact]
        public void Parse_RecordTypeSchema_HasTypeDocumentation()
        {
            var schema = ParseSchema("RecordType.mds");
            var type = schema.AllRecordTypes.Find("midiBigEndian");

            type.Attributes.Should().NotBeNull();
            type.Attributes.Should().NotBeEmpty();
            type.Attributes.Find("Documentation").Should().NotBeNull();
        }

        [Fact]
        public void Parse_RecordTypeSchema_HasTypeDocumentationText()
        {
            var schema = ParseSchema("RecordType.mds");
            var type = schema.AllRecordTypes.Find("midiBigEndian");

            var docAttr = type.Attributes.Find("Documentation");
            docAttr.Value.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Parse_DataTypeFromImport_SchemaWasImported()
        {
            using (var state = CreateParserTestState("DataTypeFromImport.mds"))
            {
                var schema = state.Parse();

                state.Schemas.Should().NotBeEmpty();
                state.Schemas.Find(SchemaConstants.DeviceTypesSchemaName);
            }
        }

        [Fact]
        public void Parse_DataTypeFromImport_DerivedTypeFound()
        {
            var schema = ParseSchema("DataTypeFromImport.mds");

            schema.AllDataTypes.Find("derivedDataType").Should().NotBeNull();
        }

        [Fact]
        public void Parse_DataTypeFromImport_DerivedTypeDerivesFromImportedType()
        {
            var schema = ParseSchema("DataTypeFromImport.mds");
            var type = schema.AllDataTypes.Find("derivedDataType");

            type.BaseType.Should().NotBeNull();
            type.BaseType.Name.FullName.Should().Be(SchemaConstants.DeviceType_MidiByte);
        }

        [Fact]
        public void Parse_InvalidDataType_ThrowsException()
        {
            using (var state = CreateParserTestState("InvalidDataTypeBase.mds"))
            {
                state.Invoking(p => p.Parse())
                    .Should().Throw<DeviceSchemaException>();
            }
        }

        [Fact]
        public void Parse_InvalidRecordType_Exception()
        {
            using(var state = CreateParserTestState("InvalidRecordTypeBase.mds"))
            {
                state.Invoking(p => p.Parse())
                    .Should().Throw<DeviceSchemaException>();
            }
        }

        [Fact]
        public void Parse_InvalidFieldType_Exception()
        {
            using (var state = CreateParserTestState("InvalidFieldType.mds"))
            {
                state.Invoking(p => p.Parse())
                    .Should().Throw<DeviceSchemaException>();
            }
        }
    }
}