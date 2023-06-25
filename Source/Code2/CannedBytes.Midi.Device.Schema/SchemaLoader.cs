using System;
using System.Diagnostics;
using System.IO;
using CannedBytes.Midi.Device.Schema.Xml;

namespace CannedBytes.Midi.Device.Schema;

public sealed class SchemaLoader
{
    private readonly DeviceSchemaSet _schemas;

    public SchemaLoader(DeviceSchemaSet schemas)
        => _schemas = schemas;

    public DeviceSchema LoadSchema(SchemaName schemaName)
    {
        if (schemaName.IsSingleSchema)
        {
            // not implemented!
            Debug.Assert(!schemaName.HasSchemaNamespace);

            Tracer.TraceEvent(
                System.Diagnostics.TraceEventType.Information,
                $"Provider: Opening Schema with name '{schemaName.FileName}' from assembly '{schemaName.AssemblyName}'.");

            using var stream = SchemaLocator.OpenSchemaStream(schemaName)
                ?? throw new DeviceSchemaException(
                    $"Failed to open schema stream for {schemaName.FileName} ({schemaName.AssemblyName}).");

            var schema = Parse(stream);
            return schema;
        }

        throw new InvalidOperationException(
            $"The provided {schemaName} does not identify a single DeviceSchema.");
    }

    private DeviceSchema Parse(Stream stream)
    {
        var parser = new MidiDeviceSchemaParser(_schemas);
        var schema = parser.Parse(stream);
        return schema;
    }
}
