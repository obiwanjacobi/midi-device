﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema.Xml;

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

    //public DeviceSchema Load(string schemaLocation)
    //{
    //    Assert.IfArgumentNullOrEmpty(schemaLocation, nameof(schemaLocation));

    //    var parts = schemaLocation.Split("::");

    //    string schemaAssembly = null;
    //    string schemaName;
    //    if (parts.Length == 1)
    //    {
    //        schemaName = parts[0];
    //    }
    //    else if (parts.Length == 2)
    //    {
    //        schemaAssembly = parts[0];
    //        schemaName = parts[1];
    //    }
    //    else
    //    {
    //        schemaName = schemaLocation;
    //    }

    //    Tracer.TraceEvent(
    //        System.Diagnostics.TraceEventType.Information,
    //        "Provider: Opening Schema with name '{0}' from assembly '{1}'.", schemaName, schemaAssembly);

    //    using var stream = DeviceSchemaImportResolver.OpenSchemaStream(schemaName, schemaAssembly)
    //        ?? throw new DeviceSchemaNotFoundException($"{schemaName} - {schemaAssembly}");

    //    MidiDeviceSchemaParser parser = new(_schemas);
    //    var deviceSchema = parser.Parse(stream);

    //    return deviceSchema;
    //}

    public DeviceSchema Open(SchemaName schemaName)
    {
        var deviceSchema = _schemas.Find(schemaName.SchemaNamespace!)
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