using CannedBytes.Midi.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CannedBytes.Midi.Device
{
    public class DevicePropertyCollection : Collection<DevicePropertyCollection.DeviceProperty>
    {
        public DeviceProperty Add(string schemaName, string propertyName, object value)
        {
            var property = new DeviceProperty(schemaName, propertyName, value);

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

        public class DeviceProperty
        {
            public DeviceProperty(string schemaName, string propertyName, object value)
            {
                this.SchemaName = schemaName;
                this.Name = propertyName;
                this.Value = value;
            }

            public string SchemaName { get; protected set; }

            public string Name { get; protected set; }

            protected object Value { get; set; }

            public T GetValue<T>()
            {
                var requestedType = typeof(T);

                return (T)Convert.ChangeType(this.Value, requestedType);
            }
        }
    }
}