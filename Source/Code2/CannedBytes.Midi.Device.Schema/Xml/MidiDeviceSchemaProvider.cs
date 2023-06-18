using System.Collections.Generic;
using System.IO;
using System.Linq;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaProvider : IDeviceSchemaProvider
{
    private readonly MidiDeviceSchemaSet _schemas = new();

    public IEnumerable<string> SchemaNames
    {
        get
        {
            return from schema in _schemas
                   select schema.SchemaName;
        }
    }

    public DeviceSchema Load(string schemaLocation)
    {
        Assert.IfArgumentNullOrEmpty(schemaLocation, nameof(schemaLocation));

        var parts = schemaLocation.Split("::");

        string schemaAssembly = null;
        string schemaName;
        if (parts.Length == 1)
        {
            schemaName = parts[0];
        }
        else if (parts.Length == 2)
        {
            schemaAssembly = parts[0];
            schemaName = parts[1];
        }
        else
        {
            schemaName = schemaLocation;
        }

        Tracer.TraceEvent(
            System.Diagnostics.TraceEventType.Information,
            "Provider: Opening Schema with name '{0}' from assembly '{1}'.", schemaName, schemaAssembly);

        using var stream = MidiDeviceSchemaImportResolver.OpenSchema(schemaName, schemaAssembly)
            ?? throw new DeviceSchemaNotFoundException($"{schemaName} - {schemaAssembly}");
        
        MidiDeviceSchemaParser parser = new(_schemas);
        var deviceSchema = parser.Parse(stream);

        return deviceSchema;
    }

    public DeviceSchema Open(string schemaName)
    {
        var deviceSchema = _schemas.Find(schemaName)
            ?? Load(schemaName);
        return deviceSchema;
    }

    public RecordType FindRecordType(string schemaName, string typeName)
    {
        Assert.IfArgumentNullOrEmpty(schemaName, nameof(schemaName));
        Assert.IfArgumentNullOrEmpty(typeName, nameof(typeName));

        RecordType recordType = null;
        var schema = _schemas.Find(schemaName);

        if (schema != null)
        {
            recordType = schema.AllRecordTypes.Find(typeName);
        }

        return recordType;
    }

    public DataType FindDataType(string schemaName, string typeName)
    {
        Assert.IfArgumentNullOrEmpty(schemaName, nameof(schemaName));
        Assert.IfArgumentNullOrEmpty(typeName, nameof(typeName));

        DataType dataType = null;
        var schema = _schemas.Find(schemaName);

        if (schema != null)
        {
            dataType = schema.AllDataTypes.Find(typeName);
        }

        return dataType;
    }
}