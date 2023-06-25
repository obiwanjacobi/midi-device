using System;
using System.Collections.Generic;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace TestApp.Commands;

internal class OpenDeviceSchemaCommand : Command
{
    private readonly IDeviceSchemaProvider _schemaProvider;
    private readonly IList<DeviceSchema> _schemas;

    public OpenDeviceSchemaCommand(IServiceProvider services, IList<DeviceSchema> schemas)
    {
        _schemaProvider = services.GetRequiredService<IDeviceSchemaProvider>();
        _schemas = schemas;
    }

    public override bool CanExecute(object? parameter)
    {
        return parameter is string;
    }

    public override void Execute(object? parameter)
    {
        if (parameter is string name)
        {
            var schema = _schemaProvider.Open(name);
            if (schema is not null)
            {
                _schemas.Add(schema);
            }
        }
    }

    public void LoadMidiTypes()
    {
        var schema = _schemaProvider.Open(MidiTypes.MidiTypesSchemaName);
    }
}
