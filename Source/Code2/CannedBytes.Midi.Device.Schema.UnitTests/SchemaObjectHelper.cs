using FluentAssertions;

namespace CannedBytes.Midi.Device.Schema.UnitTests;

public static class SchemaObjectHelper
{
    public static void AssertName(SchemaObject schemaObject, string schemaName, string objName)
    {
        schemaObject.Should().NotBeNull();
        schemaObject.Name.Should().NotBeNull();
        schemaObject.Name.Name.Should().BeEquivalentTo(objName);
        schemaObject.Name.SchemaName.Should().BeEquivalentTo(schemaName);
        schemaObject.Name.FullName.Should().BeEquivalentTo(schemaName + ":" + objName);
    }
}