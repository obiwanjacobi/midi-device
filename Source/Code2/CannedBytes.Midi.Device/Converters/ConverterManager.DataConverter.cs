using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private readonly Dictionary<string, DataConverter> _dataConverters = new();

    /// <summary>
    /// Returns a converter for the specified <paramref name="dataType"/>.
    /// </summary>
    /// <returns>Returns null if no converter suited for type was found.</returns>
    public DataConverter GetConverter(DataType dataType)
    {
        return GetConverter(dataType, dataType);
    }

    public DataConverter GetConverter(DataType matchType, DataType constructType)
    {
        Check.IfArgumentNull(matchType, "matchType");
        Check.IfArgumentNull(constructType, "constructType");

        // lookup matchtype (matchtype == constructtype on entry)
        // not found -> lookup factory for schema
        //   not found -> matchtype = matchtype.basetype - repeat
        // factory found: create - return converter

        DataConverter converter;
        do
        {
            converter = LookupConverter(matchType);

            if (converter == null)
            {
                converter = CreateConverter(matchType, constructType);

                if (converter != null)
                {
                    _dataConverters.Add(constructType.Name.FullName, converter);
                }
                else
                {
                    matchType = matchType.BaseType;
                }
            }
        }
        while (matchType != null && converter == null);

        return converter;
    }

    public DataConverter LookupConverter(DataType dataType)
    {
        Check.IfArgumentNull(dataType, "dataType");

        if (_dataConverters.TryGetValue(dataType.Name.FullName, out DataConverter converter))
        {
            return converter;
        }

        return null;
    }

    protected DataConverter CreateConverter(DataType matchType, DataType constructType)
    {
        Check.IfArgumentNull(matchType, "matchType");
        Check.IfArgumentNull(constructType, "constructType");

        DataConverter converter = null;
        IConverterFactory factory = _factoryMgr.Lookup(matchType.Schema.SchemaName);

        if (factory != null)
        {
            converter = factory.Create(matchType, constructType);
        }

        //if (converter == null && matchType.IsExtension)
        //{
        //    IConverterExtension extension = null;

        //    for (int i = 0; i < matchType.BaseTypes.Count; i++)
        //    {
        //        var baseType = matchType.BaseTypes[i];
        //        var innerConverter = Create(baseType, constructType);

        //        if (innerConverter != null)
        //        {
        //            var innerExtension = innerConverter as IConverterExtension;

        //            if (innerExtension != null)
        //            {
        //                if (extension != null)
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
