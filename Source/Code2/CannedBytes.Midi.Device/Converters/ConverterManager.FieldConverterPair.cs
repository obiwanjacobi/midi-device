﻿using System.Collections.Generic;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private readonly Dictionary<string, FieldConverterPair> _fieldConverterPairs = new();

    public FieldConverterPair GetFieldConverterPair(Field field)
    {
        Assert.IfArgumentNull(field, nameof(field));

        if (!TryLookupFieldConverterPair(field, out var pair))
        {
            var converter = GetConverter(field)
                ?? throw new DeviceSchemaException(
                    $"No DataConverter could be created for field '{field.Name.FullName}' with dataType '{field.DataType?.Name.FullName}'.");

            pair = new FieldConverterPair(field, converter);
            _fieldConverterPairs.Add(BuildFieldTypeKey(field), pair);
        }

        return pair;
    }

    public bool TryLookupFieldConverterPair(Field field, out FieldConverterPair fieldConverterPair)
    {
        Assert.IfArgumentNull(field, nameof(field));

        var fieldKey = BuildFieldTypeKey(field);
        if (_fieldConverterPairs.TryGetValue(fieldKey, out FieldConverterPair pair))
        {
            fieldConverterPair = pair;
            return true;
        }

        fieldConverterPair = default;
        return false;
    }

    // prevents giving out wrong pairs when field names are the same.
    private static string BuildFieldTypeKey(Field field)
    {
        if (field.DataType is not null)
        {
            return $"{field.Name.FullName}:{field.DataType.Name.FullName}";
        }

        if (field.RecordType is not null)
        {
            return $"{field.Name.FullName}:{field.RecordType.Name.FullName}";
        }

        return field.Name.FullName;
    }
}
