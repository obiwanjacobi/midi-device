using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests;

public class MidiDeviceSchemaProviderTest
{
    [Fact]
    public void Load_MidiTypesSchema_LoadedFormAssembly()
    {
        var schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypesSchema);

        schema.Should().NotBeNull();
    }
}
