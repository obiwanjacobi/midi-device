using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CannedBytes.Midi.Device.Message;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.DeviceTestApp.Midi
{
    [Export]
    class DeviceManager
    {
        [ImportMany(typeof(MessageDeviceProvider))]
        private IEnumerable<MessageDeviceProvider> providers;

        private List<MessageDeviceProvider> dynamicProviders = new List<MessageDeviceProvider>();

        public DeviceManager()
        {
            //Add("CannedBytes.Midi.Device.Roland.U220/Roland U-220.xsd", "Roland", "U-220", 65, 0x2b);
        }

        public MessageDeviceProvider Find(string manufacturer, string model)
        {
            return (from provider in this.providers.Union(this.dynamicProviders)
                    where provider.Manufacturer == manufacturer
                    where provider.ModelName == model
                    select provider).FirstOrDefault();
        }

        public MessageDeviceProvider Find(string schema)
        {
            return (from provider in this.providers.Union(this.dynamicProviders)
                    where provider.Schema != null
                    where provider.Schema.Name == schema
                    select provider).FirstOrDefault();
        }

        public IEnumerable<DeviceSchema> Schemas
        {
            get
            {
                return from provider in this.providers.Union(dynamicProviders)
                       where provider.ManufacturerId != 0
                       where provider.Schema != null
                       select provider.Schema;
            }
        }

        public MessageDeviceProvider Add(string schemaName, string manufacturer, string model, byte manId, byte modId)
        {
            var provider = new MessageDeviceProvider();
            App.Current.Container.SatisfyImportsOnce(provider);

            provider.Initialze(schemaName, manufacturer, model, manId, modId);
            dynamicProviders.Add(provider);
            return provider;
        }
    }
}