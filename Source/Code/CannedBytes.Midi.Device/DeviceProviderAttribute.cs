using System;
using System.ComponentModel.Composition;

namespace CannedBytes.Midi.Device
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DeviceProviderAttribute : ExportAttribute, IDeviceProviderInfo
    {
        public DeviceProviderAttribute(Type contractType, string manufacturer, string modelName, byte manufacturerId, byte modelId)
            : base(contractType)
        {
            Manufacturer = manufacturer;
            ModelName = modelName;
            ManufacturerId = manufacturerId;
            ModelId = modelId;
        }

        public string Manufacturer { get; private set; }

        public string ModelName { get; private set; }

        public byte ManufacturerId { get; private set; }

        public byte ModelId { get; private set; }

        public static DeviceProviderAttribute From(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(DeviceProviderAttribute), true);

            if (attrs != null && attrs.Length > 0)
            {
                return (DeviceProviderAttribute)attrs[0];
            }

            return null;
        }
    }

    public interface IDeviceProviderInfo
    {
        string Manufacturer { get; }

        string ModelName { get; }

        byte ManufacturerId { get; }

        byte ModelId { get; }
    }
}