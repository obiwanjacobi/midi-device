using CannedBytes.Midi.Device.Schema;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Converters
{
    partial class ConverterManager
    {
        private readonly Dictionary<string, FieldConverterPair> _fieldConverterPairs = 
            new Dictionary<string, FieldConverterPair>();

        public FieldConverterPair GetFieldConverterPair(Field field)
        {
            Check.IfArgumentNull(field, "field");

            var pair = LookupFieldConverterPair(field);

            if (pair == null)
            {
                var converter = GetConverter(field);

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
            Check.IfArgumentNull(field, "field");

            var fieldKey = BuildFieldTypeKey(field);

            FieldConverterPair pair = null;

            if (_fieldConverterPairs.TryGetValue(fieldKey, out pair))
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
                return field.Name.FullName + ":" + field.DataType.Name.FullName;
            }

            if (field.RecordType != null)
            {
                return field.Name.FullName + ":" + field.RecordType.Name.FullName;
            }

            return field.Name.FullName;
        }
    }
}
