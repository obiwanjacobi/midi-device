using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private readonly Dictionary<string, FieldConverterPair> _fieldConverterPairs = new();

    public FieldConverterPair GetFieldConverterPair(Field field)
    {
        Check.IfArgumentNull(field, nameof(field));

        FieldConverterPair pair = LookupFieldConverterPair(field);

        if (pair == null)
        {
            IConverter converter = GetConverter(field);

            if (converter != null)
            {
                pair = new FieldConverterPair(field, converter);

                _fieldConverterPairs.Add(BuildFieldTypeKey(field), pair);
            }
        }

        return pair;
    }

    public FieldConverterPair LookupFieldConverterPair(Field field)
    {
        Check.IfArgumentNull(field, nameof(field));

        string fieldKey = BuildFieldTypeKey(field);
        if (_fieldConverterPairs.TryGetValue(fieldKey, out FieldConverterPair pair))
        {
            return pair;
        }

        return null;
    }

    // prevents giving out wrong pairs when field names are the same.
    private static string BuildFieldTypeKey(Field field)
    {
        if (field.DataType != null)
        {
            return $"{field.Name.FullName}:{field.DataType.Name.FullName}";
        }

        if (field.RecordType != null)
        {
            return $"{field.Name.FullName}:{field.RecordType.Name.FullName}";
        }

        return field.Name.FullName;
    }
}
