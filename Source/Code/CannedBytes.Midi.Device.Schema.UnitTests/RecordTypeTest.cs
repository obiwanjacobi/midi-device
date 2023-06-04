using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.RecordType and is intended
    ///to contain all Jacobi.Midi.Device.Schema.RecordType Unit Tests
    ///</summary>
    
    public class RecordTypeTest
    {
        private const string SchemaName = "urn:midi-test-schema";
        private const string TypeName = "TestType";

        [Fact]
        public void RecordType_ConstructorTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            RecordType target = new RecordType(fullName);

            SchemaObjectTest.AssertName(target, SchemaName, TypeName);
        }

        [Fact]
        public void RecordType_FieldTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            RecordType target = new RecordType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            target.Fields.Should().NotBeNull();

            string fieldName = SchemaName + ":TestField";

            Field field = new Field(fieldName);
            SchemaObjectTest.AssertName(field, SchemaName, "TestField");

            target.Fields.Add(field);

            target.Fields.Should().HaveCount(1);
            target.Fields[0].Should().NotBeNull();
            target.Fields[0].Should().Be(field);
        }
    }
}