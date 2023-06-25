using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests;

public class MidiDeviceSchemaProviderTest
{
    [Fact]
    public void Load_MidiTypesSchema_LoadedFormAssembly()
    {
        Schema.DeviceSchema schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypes);

        schema.Should().NotBeNull();
    }
}
