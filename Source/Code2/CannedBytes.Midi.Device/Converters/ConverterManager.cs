using System.Collections.Generic;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

public partial class ConverterManager
{
    private readonly FactoryManager _factoryMgr;

    public ConverterManager(IEnumerable<IConverterFactory> factories)
    {
        _factoryMgr = new FactoryManager(factories);
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
