using System;
using System.IO;
using System.Linq;
using CannedBytes.Midi.Device.Schema.Xml;
using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests.Xml;

public class MidiDeviceSchemaParserTest
{
    private const string Folder = "Xml";
    private const string DataType = "DataType.mds";
    private const string RecordType = "RecordType.mds";
    private const string DataTypeConstraints = "DataTypeConstraints.mds";
    private const string DeviceSchemaTypes = "DeviceSchemaTypes.mds";
    private const string DataTypeFromImport = "DataTypeFromImport.mds";
    private const string DeviceSchema3 = "DeviceSchema3.mds";
    private const string InvalidDataTypeBase = "InvalidDataTypeBase.mds";
    private const string InvalidRecordTypeBase = "InvalidRecordTypeBase.mds";
    private const string InvalidFieldType = "InvalidFieldType.mds";

    public sealed class ParserTestState : IDisposable
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

    public static ParserTestState CreateParserTestState(string schema)
    {
        var path = Path.Combine(Folder, schema);
        var schemas = new MidiDeviceSchemaSet();
        
        return new ParserTestState
        {
            Schemas = schemas,
            Parser = new MidiDeviceSchemaParser(schemas),
            Stream = File.OpenRead(path)
        };
    }

    public static DeviceSchema ParseSchema(string schema)
    {
        using var state = CreateParserTestState(schema);
        return state.Parse();
    }


    // TODO: Move this to CannedBytes.Midi.Device.IntegrationTests
    //[Fact]
    public void Parse_ImportResource_NoErrors()
    {
        var schemas = new MidiDeviceSchemaSet();
        var parser = new MidiDeviceSchemaParser(schemas);

        using var stream = File.OpenRead(DeviceSchema3);
        var schema = parser.Parse(stream);

        schemas.Find("http://schemas.cannedbytes.com/midi-device-schema/midi-types/10")
            .Should().NotBeNull();

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


    [Fact]
    public void Parse_DeviceSchemaTypes_ReturnsInstance()
    {
        var schema = ParseSchema(DeviceSchemaTypes);

        schema.Should().NotBeNull();
    }

    [Fact]
    public void Parse_DeviceSchemaTypes_HasSchemaDocumentation()
    {
        var schema = ParseSchema(DeviceSchemaTypes);

        schema.Attributes.Should().NotBeEmpty();
        schema.Attributes.Find("Documentation").Should().NotBeNull();
    }

    [Fact]
    public void Parse_DeviceSchemaTypes_HasSchemaDocumentationText()
    {
        var schema = ParseSchema(DeviceSchemaTypes);

        var docAttr = schema.Attributes.Find("Documentation");
        docAttr.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Parse_DataTypeSchema_ReturnsInstance()
    {
        var schema = ParseSchema(DataType);

        schema.Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeSchema_ContainsDataTypes()
    {
        var schema = ParseSchema(DataType);

        schema.AllDataTypes.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_DataTypeSchema_ContainsRootDataType()
    {
        var schema = ParseSchema(DataType);

        schema.AllDataTypes.Find("midiByte").Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeSchema_ContainsDerivedDataType()
    {
        var schema = ParseSchema(DataType);

        schema.AllDataTypes.Find("midiData").Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeSchema_HasTypeDocumentation()
    {
        var schema = ParseSchema(DataType);
        var type = schema.AllDataTypes.Find("midiByte");
        
        type.Attributes.Should().NotBeNull();
        type.Attributes.Should().NotBeEmpty();
        type.Attributes.Find("Documentation").Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeSchema_HasTypeDocumentationText()
    {
        var schema = ParseSchema(DataType);
        var type = schema.AllDataTypes.Find("midiByte");

        var docAttr = type.Attributes.Find("Documentation");
        docAttr.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasTypeWithConstraints()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");
        
        type.Should().NotBeNull();
        type.Constraints.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasMinConstraint()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        type.Constraints.Find(ConstraintTypes.MinInclusive).Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_MinConstraintValidates()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        var constraint = type.Constraints.Find(ConstraintTypes.MinInclusive);
        constraint.Validate(0).Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_MinConstraintValidationFails()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        var constraint = type.Constraints.Find(ConstraintTypes.MinInclusive);
        constraint.Validate(-1).Should().BeFalse();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasMaxConstraint()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        type.Constraints.Find(ConstraintTypes.MaxInclusive).Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_MaxConstraintValidates()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        var constraint = type.Constraints.Find(ConstraintTypes.MaxInclusive);
        constraint.Validate(127).Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_MaxConstraintValidationFails()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiData");

        var constraint = type.Constraints.Find(ConstraintTypes.MaxInclusive);
        constraint.Validate(128).Should().BeFalse();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasEnumConstraint()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiStatus");

        type.Constraints.Find(ConstraintTypes.Enumeration).Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_EnumConstraintValidatesFirst()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiStatus");

        var constraint = type.Constraints.FindAll(ConstraintTypes.Enumeration).First();
        constraint.Validate(8).Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_EnumConstraintValidatesLast()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiStatus");

        var constraint = type.Constraints.FindAll(ConstraintTypes.Enumeration).Last();
        constraint.Validate(15).Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_EnumConstraintValidationFails()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiStatus");

        var constraint = type.Constraints.Find(ConstraintTypes.Enumeration);
        constraint.Validate(0).Should().BeFalse();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasLengthConstraint()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiString");

        type.Constraints.Find(ConstraintTypes.FixedLength).Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_LengthConstraintValidates()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiString");

        var constraint = type.Constraints.Find(ConstraintTypes.FixedLength);
        constraint.Validate("0123456789").Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_LengthConstraintValidationFails()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("midiString");

        var constraint = type.Constraints.Find(ConstraintTypes.FixedLength);
        constraint.Validate("01234567890").Should().BeFalse();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_HasFixedConstraint()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("modelId");

        type.Constraints.Find(ConstraintTypes.FixedValue).Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_FixedConstraintValidates()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("modelId");

        var constraint = type.Constraints.Find(ConstraintTypes.FixedValue);
        constraint.Validate(16).Should().BeTrue();
    }

    [Fact]
    public void Parse_DataTypeConstraintSchema_FixedConstraintValidationFails()
    {
        var schema = ParseSchema(DataTypeConstraints);
        var type = schema.AllDataTypes.Find("modelId");

        var constraint = type.Constraints.Find(ConstraintTypes.FixedValue);
        constraint.Validate(0).Should().BeFalse();
    }

    [Fact]
    public void Parse_RecordTypeSchema_ReturnsInstance()
    {
        var schema = ParseSchema(RecordType);

        schema.Should().NotBeNull();
    }

    [Fact]
    public void Parse_RecordTypeSchema_ContainsRecordTypes()
    {
        var schema = ParseSchema(RecordType);

        schema.AllRecordTypes.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_RecordTypeSchema_ContainsVirtualRootFields()
    {
        var schema = ParseSchema(RecordType);

        schema.VirtualRootFields.Should().HaveCount(schema.RootRecordTypes.Count);
    }

    [Fact]
    public void Parse_RecordTypeSchema_ContainsRootRecordType()
    {
        var schema = ParseSchema(RecordType);

        schema.AllRecordTypes.Find("midiBigEndian").Should().NotBeNull();
    }

    [Fact]
    public void Parse_RecordTypeSchema_ContainsDerivedRecordType()
    {
        var schema = ParseSchema(RecordType);

        schema.AllRecordTypes.Find("midiSplitNibbleBE").Should().NotBeNull();
    }

    [Fact]
    public void Parse_RecordTypeSchema_HasTypeDocumentation()
    {
        var schema = ParseSchema(RecordType);
        var type = schema.AllRecordTypes.Find("midiBigEndian");

        type.Attributes.Should().NotBeNull();
        type.Attributes.Should().NotBeEmpty();
        type.Attributes.Find("Documentation").Should().NotBeNull();
    }

    [Fact]
    public void Parse_RecordTypeSchema_HasTypeDocumentationText()
    {
        var schema = ParseSchema(RecordType);
        var type = schema.AllRecordTypes.Find("midiBigEndian");

        var docAttr = type.Attributes.Find("Documentation");
        docAttr.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Parse_DataTypeFromImport_SchemaWasImported()
    {
        using var state = CreateParserTestState(DataTypeFromImport);
        var depSchema = ParseSchema(DeviceSchemaTypes);
        state.Schemas.Add(depSchema);

        var schema = state.Parse();

        state.Schemas.Should().NotBeEmpty();
        state.Schemas.Find(SchemaConstants.DeviceTypesSchemaName);
    }

    [Fact]
    public void Parse_DataTypeFromImport_DerivedTypeFound()
    {
        using var state = CreateParserTestState(DataTypeFromImport);
        var depSchema = ParseSchema(DeviceSchemaTypes);
        state.Schemas.Add(depSchema);
        var schema = state.Parse();

        schema.AllDataTypes.Find("derivedDataType").Should().NotBeNull();
    }

    [Fact]
    public void Parse_DataTypeFromImport_DerivedTypeDerivesFromImportedType()
    {
        using var state = CreateParserTestState(DataTypeFromImport);
        var depSchema = ParseSchema(DeviceSchemaTypes);
        state.Schemas.Add(depSchema);

        var schema = state.Parse();
        var type = schema.AllDataTypes.Find("derivedDataType");

        type.BaseType.Should().NotBeNull();
        type.BaseType.Name.FullName.Should().Be(SchemaConstants.DeviceType_MidiByte);
    }

    [Fact]
    public void Parse_InvalidDataType_ThrowsException()
    {
        using var state = CreateParserTestState(InvalidDataTypeBase);
        state.Invoking(p => p.Parse())
            .Should().Throw<DeviceSchemaException>();
    }

    [Fact]
    public void Parse_InvalidRecordType_Exception()
    {
        using var state = CreateParserTestState(InvalidRecordTypeBase);
        state.Invoking(p => p.Parse())
            .Should().Throw<DeviceSchemaException>();
    }

    [Fact]
    public void Parse_InvalidFieldType_Exception()
    {
        using var state = CreateParserTestState(InvalidFieldType);
        state.Invoking(p => p.Parse())
            .Should().Throw<DeviceSchemaException>();
    }
}