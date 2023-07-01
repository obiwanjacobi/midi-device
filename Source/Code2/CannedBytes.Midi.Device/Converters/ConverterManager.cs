using System.Collections.Generic;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

public sealed partial class ConverterManager
{
    private readonly FactoryManager _factoryMgr;

    public ConverterManager(IEnumerable<IConverterFactory> factories)
    {
        _factoryMgr = new FactoryManager(factories);
    }

    public IConverter? GetConverter(Field field)
    {
        Assert.IfArgumentNull(field, nameof(field));

        if (field.DataType is not null)
        {
            return GetConverter(field.DataType);
        }

        if (field.RecordType is not null)
        {
            return GetConverter(field.RecordType);
        }

        return null;
    }
}
