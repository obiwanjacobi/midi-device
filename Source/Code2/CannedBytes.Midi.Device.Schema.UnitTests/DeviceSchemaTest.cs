using Xunit;
using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    /// This is a test class for Jacobi.Midi.Device.Schema.DeviceSchema and is intended
    /// to contain all Jacobi.Midi.Device.Schema.DeviceSchema Unit Tests
    /// </summary>
    
    public class DeviceSchemaTest
    {
        [Fact]
        public void Constructor_WithName_Sticks()
        {
            var target = new DeviceSchema(Constants.SchemaName);

            target.SchemaName.Should().Be(Constants.SchemaName);
        }

        [Fact]
        public void Attributes_OnNewInstance_IsNotNull()
        {
            var target = new DeviceSchema(Constants.SchemaName);

            target.Attributes.Should().NotBeNull();
        }

        [Fact]
        public void Attributes_OnNewInstance_HasSchemaSet()
        {
            var target = new DeviceSchema(Constants.SchemaName);

            target.Attributes.Schema.Should().NotBeNull();
        }

        [Fact]
        public void AllDataTypes_OnNewInstance_IsNotNull()
        {
            var target = new DeviceSchema(Constants.SchemaName);

            target.AllDataTypes.Should().NotBeNull();
        }

        [Fact]
        public void Add_AllDataTypes_Sticks()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new DataType(Constants.SchemaTypeName);

            target.AllDataTypes.Add(type);

            target.AllDataTypes.Should().HaveCount(1);
            target.AllDataTypes.Should().NotContainNulls();
            target.AllDataTypes[0].Should().BeSameAs(type);
        }

        [Fact]
        public void Clear_AllDataTypes_IsEmpty()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new DataType(Constants.SchemaTypeName);

            target.AllDataTypes.Add(type);
            target.AllDataTypes.Clear();

            target.AllDataTypes.Should().BeEmpty();
        }

        [Fact]
        public void Add_AllRecordTypes_Sticks()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new RecordType(Constants.SchemaTypeName);

            target.AllRecordTypes.Add(type);

            target.AllRecordTypes.Should().HaveCount(1);
            target.AllRecordTypes.Should().NotContainNulls();
            target.AllRecordTypes.Should().Contain(type);
        }

        [Fact]
        public void Clear_AllRecordTypes_IsEmpty()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new RecordType(Constants.SchemaTypeName);

            target.AllRecordTypes.Add(type);
            target.AllRecordTypes.Clear();

            target.AllRecordTypes.Should().BeEmpty();
        }

        [Fact]
        public void Add_RootRecordTypes_Sticks()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new RecordType(Constants.SchemaTypeName);

            target.RootRecordTypes.Add(type);

            target.RootRecordTypes.Should().HaveCount(1);
            target.RootRecordTypes.Should().NotContainNulls();
            target.RootRecordTypes.Should().Contain(type);
        }

        [Fact]
        public void Clear_RootRecordTypes_IsEmpty()
        {
            var target = new DeviceSchema(Constants.SchemaName);
            var type = new RecordType(Constants.SchemaTypeName);

            target.RootRecordTypes.Add(type);
            target.RootRecordTypes.Clear();

            target.RootRecordTypes.Should().BeEmpty();
        }
    }
}