﻿using System.Collections.Generic;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private readonly Dictionary<string, DataConverter> _dataConverters = new();

    /// <summary>
    /// Returns a converter for the specified <paramref name="dataType"/>.
    /// </summary>
    /// <returns>Returns null if no converter suited for type was found.</returns>
    public DataConverter? GetConverter(DataType dataType)
    {
        return GetConverter(dataType, dataType);
    }

    public DataConverter? GetConverter(DataType matchType, DataType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        // lookup matchtype (matchtype == constructtype on entry)
        // not found -> lookup factory for schema
        //   not found -> matchtype = matchtype.basetype - repeat
        // factory found: create - return converter

        DataConverter? converter;
        do
        {
            converter = LookupConverter(matchType);

            if (converter is null)
            {
                converter = CreateConverter(matchType, constructType);

                if (converter is not null)
                {
                    _dataConverters.Add(constructType.Name.FullName, converter);
                }
                else
                {
                    matchType = matchType.BaseType!;
                }
            }
        }
        while (matchType is not null && converter is null);

        return converter;
    }

    public DataConverter? LookupConverter(DataType dataType)
    {
        Assert.IfArgumentNull(dataType, nameof(dataType));

        if (_dataConverters.TryGetValue(dataType.Name.FullName, out var converter))
        {
            return converter;
        }

        return null;
    }

    private DataConverter? CreateConverter(DataType matchType, DataType constructType)
    {
        Assert.IfArgumentNull(matchType, nameof(matchType));
        Assert.IfArgumentNull(constructType, nameof(constructType));

        DataConverter? converter = null;
        var factory = _factoryMgr.Lookup(matchType.Schema.SchemaName);

        if (factory is not null)
        {
            converter = factory.Create(matchType, constructType);
        }

        //if (converter is null && matchType.IsExtension)
        //{
        //    IConverterExtension extension = null;

        //    for (int i = 0; i < matchType.BaseTypes.Count; i++)
        //    {
        //        var baseType = matchType.BaseTypes[i];
        //        var innerConverter = Create(baseType, constructType);

        //        if (innerConverter is not null)
        //        {
        //            var innerExtension = innerConverter as IConverterExtension;

        //            if (innerExtension is not null)
        //            {
        //                if (extension is not null)
        //                {
        //                    extension.InnerConverter = innerExtension;
        //                }
        //                else
        //                {
        //                    converter = innerConverter;
        //                }

        //                extension = innerExtension;
        //            }
        //            else
        //            {
        //                throw new DeviceDataException(String.Format(
        //                    "Converter '{0}' does not support extensions but is used with the DataType '{1}' that is an extension.",
        //                        innerConverter.GetType().FullName, baseType.Name));
        //            }
        //        }
        //        else
        //        {
        //            throw new DeviceDataException(String.Format(
        //                "No Converter was found for the DataType '{0}'.", baseType.Name));
        //        }
        //    }
        //}

        return converter;
    }
}
