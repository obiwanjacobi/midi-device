using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.DeviceSchema and is intended
    ///to contain all Jacobi.Midi.Device.Schema.DeviceSchema Unit Tests
    ///</summary>
    
    public class DeviceSchemaTest
    {
        private const string SchemaName = "urn:midi-test-schema";

        [Fact]
        public void DeviceSchema_ConstructorTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            target.SchemaName.Should().Be(SchemaName);
        }

        [Fact]
        public void DeviceSchema_DataTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            target.SchemaName.Should().Be(SchemaName);
            target.AllDataTypes.Should().NotBeNull();

            string typeName = SchemaName + ":DataType";
            DataType type = new DataType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            type.Schema.Should().BeNull();

            target.AllDataTypes.Add(type);

            type.Schema.Should().NotBeNull();
            type.Schema.Should().Be(target);
            target.AllDataTypes.Should().HaveCount(1);
            target.AllDataTypes[0].Should().NotBeNull();
            target.AllDataTypes[0].Should().Be(type);

            target.AllDataTypes.Clear();

            target.AllDataTypes.Should().BeEmpty();
        }

        [Fact]
        public void DeviceSchema_RecordTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            target.SchemaName.Should().Be(SchemaName);
            target.AllDataTypes.Should().NotBeNull();

            string typeName = SchemaName + ":DataType";
            RecordType type = new RecordType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            type.Schema.Should().BeNull();

            target.AllRecordTypes.Add(type);

            type.Schema.Should().NotBeNull();
            type.Schema.Should().Be(target);
            target.AllRecordTypes.Should().HaveCount(1);
            target.AllRecordTypes[0].Should().NotBeNull();
            target.AllRecordTypes[0].Should().Be(type);

            target.AllRecordTypes.Clear();

            target.AllRecordTypes.Should().BeEmpty();
        }

        [Fact]
        public void DeviceSchema_RootTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            target.SchemaName.Should().Be(SchemaName);
            target.AllDataTypes.Should().NotBeNull();

            string typeName = SchemaName + ":DataType";
            RecordType type = new RecordType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            type.Schema.Should().BeNull();

            target.RootRecordTypes.Add(type);

            type.Schema.Should().NotBeNull();
            type.Schema.Should().Be(target);
            target.AllRecordTypes.Should().HaveCount(1);
            target.AllRecordTypes[0].Should().NotBeNull();
            target.AllRecordTypes[0].Should().Be(type);

            target.RootRecordTypes.Clear();

            target.AllRecordTypes.Should().BeEmpty();
        }
    }
}