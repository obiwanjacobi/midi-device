using System.Collections.Generic;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private readonly Dictionary<string, StreamConverter> _streamConverters = new();

    /// <summary>
    /// Returns a converter container for the specified <paramref name="recordType"/>.
    /// </summary>
    /// <returns>Returns null if no converter suited for type was found.</returns>
    public StreamConverter GetConverter(RecordType recordType)
    {
        return GetConverter(recordType, recordType);
    }

    public StreamConverter GetConverter(RecordType matchType, RecordType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        // lookup matchtype (matchtype == constructtype on entry)
        // not found -> lookup factory for schema
        //   not found -> matchtype = matchtype.basetype - repeat
        // factory found: create - return converter

        StreamConverter converter;

        do
        {
            converter = LookupConverter(matchType);

            if (converter == null)
            {
                converter = CreateConverter(matchType, constructType);

                if (converter != null)
                {
                    if (!IsDynamic(constructType))
                    {
                        _streamConverters.Add(constructType.Name.FullName, converter);
                    }
                }
                else
                {
                    matchType = matchType.BaseType;
                }
            }
        }
        while (matchType != null && converter == null);

        // always create a StreamConverter for a RecordType.
        converter ??= _factoryMgr.DefaultFactory.Create(constructType, constructType);

        return converter;
    }

    public StreamConverter LookupConverter(RecordType recordType)
    {
        Assert.IfArgumentNull(recordType, nameof(recordType));

        if (_streamConverters.TryGetValue(recordType.Name.FullName, out StreamConverter converter))
        {
            return converter;
        }

        return null;
    }

    private static bool IsDynamic(RecordType constructType)
    {
        Assert.IfArgumentNull(constructType, nameof(constructType));

        if (!constructType.IsDynamic)
        {
            // do we need to check children!?

            //foreach (var field in constructType.Fields)
            //{
            //    if (field.RecordType != null)
            //    {
            //        if (IsDynamic(field.RecordType))
            //        {
            //            return true;
            //        }
            //    }
            //}

            return false;
        }

        return true;
    }

    private StreamConverter CreateConverter(RecordType matchType, RecordType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        var factory = _factoryMgr.Lookup(matchType.Schema.SchemaName);

        StreamConverter converter = null;
        if (factory != null)
        {
            converter = factory.Create(matchType, constructType);
        }

        return converter;
    }
}
