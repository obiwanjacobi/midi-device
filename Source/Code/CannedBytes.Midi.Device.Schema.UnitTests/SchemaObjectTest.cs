using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    /// Summary description for SchemaObjectTest
    /// </summary>
    
    public static class SchemaObjectTest
    {
        public static void AssertName(SchemaObject schemaObject, string schemaName, string objName)
        {
            schemaObject.Should().NotBeNull();
            schemaObject.Name.Should().NotBeNull();
            schemaObject.Name.SchemaName.Should().Be(schemaName);
            schemaObject.Name.Name.Should().Be(objName);
            schemaObject.Name.FullName.Should().Be(schemaName + ":" + objName);
        }
    }
}