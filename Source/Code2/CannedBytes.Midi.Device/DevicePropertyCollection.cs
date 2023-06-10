using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CannedBytes.Midi.Device;

public class DevicePropertyCollection : Collection<DeviceProperty>
{
    public DeviceProperty Add(string schemaName, string propertyName, object value)
    {
        DeviceProperty property = new(schemaName, propertyName, value);

        Add(property);

        return property;
    }

    public DeviceProperty Find(string propertyName)
    {
        return (from property in this
                where property.Name == propertyName
                select property).FirstOrDefault();
    }

    public DeviceProperty Find(string schemaName, string propertyName)
    {
        return (from property in this
                where property.SchemaName == schemaName
                where property.Name == propertyName
                select property).FirstOrDefault();
    }

    public IEnumerable<DeviceProperty> FindAll(string schemaName)
    {
        return from property in this
               where property.SchemaName == schemaName
               select property;
    }
}