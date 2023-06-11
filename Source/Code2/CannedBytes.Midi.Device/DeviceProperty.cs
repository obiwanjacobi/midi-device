﻿using System;

namespace CannedBytes.Midi.Device;

public class DeviceProperty
{
    /// <summary>Used with the 'property' attribute in a schema to indicate the field that contains the address of an address-map message.</summary>
    public const string AddressPropertyName = "address";
    /// <summary>Used with the 'property' attribute in a schema to indicate the field that contains the size (or length) of an address-map message.</summary>
    public const string SizePropertyName = "size";

    public DeviceProperty(string schemaName, string propertyName, object value)
    {
        SchemaName = schemaName;
        Name = propertyName;
        Value = value;
    }

    public string SchemaName { get; protected set; }

    public string Name { get; protected set; }

    protected object Value { get; set; }

    public T GetValue<T>()
    {
        var requestedType = typeof(T);

        return (T)Convert.ChangeType(Value, requestedType);
    }
}
