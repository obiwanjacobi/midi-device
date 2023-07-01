using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

public sealed class DeviceSchemaProvider : IDeviceSchemaProvider
{
    private readonly DeviceSchemaSet _schemas = new();

    public IEnumerable<string> SchemaNames
    {
        get
        {
            return from schema in _schemas
                   select schema.SchemaName;
        }
    }

    public DeviceSchema Load(SchemaName schemaName)
    {
        var loader = new SchemaLoader(_schemas);
        var deviceSchema = loader.LoadSchema(schemaName);
        return deviceSchema;
    }

    public DeviceSchema Open(SchemaName schemaName)
    {
        DeviceSchema? deviceSchema = null;

        if (schemaName.HasSchemaNamespace)
            deviceSchema = _schemas.Find(schemaName.SchemaNamespace!);

        return deviceSchema ?? Load(schemaName);
    }

    public RecordType? FindRecordType(string schemaName, string typeName)
    {
        Assert.IfArgumentNullOrEmpty(schemaName, nameof(schemaName));
        Assert.IfArgumentNullOrEmpty(typeName, nameof(typeName));

        RecordType? recordType = null;
        var schema = _schemas.Find(schemaName);

        if (schema is not null)
        {
            recordType = schema.AllRecordTypes.Find(typeName);
        }

        return recordType;
    }

    public DataType? FindDataType(string schemaName, string typeName)
    {
        Assert.IfArgumentNullOrEmpty(schemaName, nameof(schemaName));
        Assert.IfArgumentNullOrEmpty(typeName, nameof(typeName));

        DataType? dataType = null;
        var schema = _schemas.Find(schemaName);

        if (schema is not null)
        {
            dataType = schema.AllDataTypes.Find(typeName);
        }

        return dataType;
    }
}