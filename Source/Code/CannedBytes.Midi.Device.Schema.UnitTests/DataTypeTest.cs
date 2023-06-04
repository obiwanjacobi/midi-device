using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.DataType and is intended
    ///to contain all Jacobi.Midi.Device.Schema.DataType Unit Tests
    ///</summary>
    
    public class DataTypeTest
    {
        private const string SchemaName = "urn:midi-test-schema";
        private const string TypeName = "TestType";

        [Fact]
        public void DataType_ConstructorTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);

            SchemaObjectTest.AssertName(target, SchemaName, TypeName);
        }

        [Fact]
        public void DataType_BaseTypeTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            DataType baseType = new DataType(SchemaName + ":BaseType");
            SchemaObjectTest.AssertName(baseType, SchemaName, "BaseType");

            target.BaseTypes.Add(baseType);

            target.HasBaseTypes.Should().BeTrue();
            target.BaseType.Should().NotBeNull();
            target.BaseType.Should().Be(baseType);
            target.BaseTypes.Should().HaveCount(1);
        }

        [Fact]
        public void DataType_IsTypeTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            DataType baseType = new DataType(SchemaName + ":BaseType");
            SchemaObjectTest.AssertName(baseType, SchemaName, "BaseType");

            target.BaseTypes.Add(baseType);

            // own type should be found with/w-out recursive.
            target.IsType(fullName, false).Should().BeTrue();
            target.IsType(fullName, true).Should().BeTrue();
            // immediate base type should be found with/w-out recursive.
            target.IsType(SchemaName + ":BaseType", true).Should().BeTrue();
            target.IsType(SchemaName + ":BaseType", false).Should().BeTrue();
        }
    }
}