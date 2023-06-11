﻿using System;
using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

//[Export]
public partial class ConverterManager
{
    private readonly FactoryManager _factoryMgr;

    //[ImportingConstructor]
    public ConverterManager(
        //[Import]
        AttributedConverterFactory attributedFactory,
        //[ImportMany(typeof(IConverterFactory))]
        //IEnumerable<Lazy<IConverterFactory, IConverterFactoryInfo>> factories
        IEnumerable<IConverterFactory> factories)
    {
        _factoryMgr = new FactoryManager(attributedFactory, factories);
    }

    public IConverter GetConverter(Field field)
    {
        Check.IfArgumentNull(field, "field");

        if (field.DataType != null)
        {
            return GetConverter(field.DataType);
        }

        if (field.RecordType != null)
        {
            return GetConverter(field.RecordType);
        }

        return null;
    }
}
