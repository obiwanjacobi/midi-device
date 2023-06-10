using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests;

/// <summary>
/// This is a test class for Jacobi.Midi.Device.Schema.DeviceSchema and is intended
/// to contain all Jacobi.Midi.Device.Schema.DeviceSchema Unit Tests
/// </summary>

public class DeviceSchemaTest
{
    [Fact]
    public void Constructor_WithName_Sticks()
    {
        DeviceSchema target = new(Constants.SchemaName);

        target.SchemaName.Should().Be(Constants.SchemaName);
    }

    [Fact]
    public void Attributes_OnNewInstance_IsNotNull()
    {
        DeviceSchema target = new(Constants.SchemaName);

        target.Attributes.Should().NotBeNull();
    }

    [Fact]
    public void Attributes_OnNewInstance_HasSchemaSet()
    {
        DeviceSchema target = new(Constants.SchemaName);

        target.Attributes.Schema.Should().NotBeNull();
    }

    [Fact]
    public void AllDataTypes_OnNewInstance_IsNotNull()
    {
        DeviceSchema target = new(Constants.SchemaName);

        target.AllDataTypes.Should().NotBeNull();
    }

    [Fact]
    public void Add_AllDataTypes_Sticks()
    {
        DeviceSchema target = new(Constants.SchemaName);
        DataType type = new(Constants.SchemaTypeName);

        target.AllDataTypes.Add(type);

        target.AllDataTypes.Should().HaveCount(1);
        target.AllDataTypes.Should().NotContainNulls();
        target.AllDataTypes[0].Should().BeSameAs(type);
    }

    [Fact]
    public void Clear_AllDataTypes_IsEmpty()
    {
        DeviceSchema target = new(Constants.SchemaName);
        DataType type = new(Constants.SchemaTypeName);

        target.AllDataTypes.Add(type);
        target.AllDataTypes.Clear();

        target.AllDataTypes.Should().BeEmpty();
    }

    [Fact]
    public void Add_AllRecordTypes_Sticks()
    {
        DeviceSchema target = new(Constants.SchemaName);
        RecordType type = new(Constants.SchemaTypeName);

        target.AllRecordTypes.Add(type);

        target.AllRecordTypes.Should().HaveCount(1);
        target.AllRecordTypes.Should().NotContainNulls();
        target.AllRecordTypes.Should().Contain(type);
    }

    [Fact]
    public void Clear_AllRecordTypes_IsEmpty()
    {
        DeviceSchema target = new(Constants.SchemaName);
        RecordType type = new(Constants.SchemaTypeName);

        target.AllRecordTypes.Add(type);
        target.AllRecordTypes.Clear();

        target.AllRecordTypes.Should().BeEmpty();
    }

    [Fact]
    public void Add_RootRecordTypes_Sticks()
    {
        DeviceSchema target = new(Constants.SchemaName);
        RecordType type = new(Constants.SchemaTypeName);

        target.RootRecordTypes.Add(type);

        target.RootRecordTypes.Should().HaveCount(1);
        target.RootRecordTypes.Should().NotContainNulls();
        target.RootRecordTypes.Should().Contain(type);
    }

    [Fact]
    public void Clear_RootRecordTypes_IsEmpty()
    {
        DeviceSchema target = new(Constants.SchemaName);
        RecordType type = new(Constants.SchemaTypeName);

        target.RootRecordTypes.Add(type);
        target.RootRecordTypes.Clear();

        target.RootRecordTypes.Should().BeEmpty();
    }
}