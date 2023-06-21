using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests;

/// <summary>
/// This is a test class for Jacobi.Midi.Device.Schema.RecordType and is intended
/// to contain all Jacobi.Midi.Device.Schema.RecordType Unit Tests
/// </summary>

public class RecordTypeTest
{
    private static DeviceSchema TestSchema = new("RecordTypeTestSchema");

    [Fact]
    public void Constructor_WithName_ParsedCorrectly()
    {
        RecordType target = new(TestSchema, Constants.SchemaTypeName);

        SchemaObjectHelper.AssertName(target, Constants.SchemaName, Constants.TypeName);
    }

    [Fact]
    public void Fields_OnNewRecordType_IsNotNull()
    {
        RecordType target = new(TestSchema, Constants.SchemaTypeName);

        target.Fields.Should().NotBeNull();
    }

    [Fact]
    public void FieldsAdd_NewField_Sticks()
    {
        RecordType target = new(TestSchema, Constants.SchemaTypeName);
        Field field = new(TestSchema, Constants.SchemaFieldName);

        target.Fields.Add(field);

        target.Fields.Should().HaveCount(1);
        target.Fields[0].Should().NotBeNull();
        target.Fields[0].Should().BeSameAs(field);
    }

    [Fact]
    public void RecordTypeSchema_SchemaProperty_IsSame()
    {
        DeviceSchema schema = new(Constants.SchemaName);
        RecordType target = new(schema, Constants.SchemaTypeName);

        // should not throw
        schema.AllRecordTypes.Add(target);

        target.Schema.Should().NotBeNull();
        target.Schema.Should().BeSameAs(schema);
    }
}