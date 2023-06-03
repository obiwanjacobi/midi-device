using CannedBytes.Midi.Device.Message;

namespace CannedBytes.Midi.Device.Roland
{
    public abstract class RolandDeviceProvider : MessageDeviceProvider
    {
        protected RolandDeviceProvider(string schemaResourceName)
        {
            if (schemaResourceName.IndexOf('/') == -1)
            {
                var type = GetType();
                SchemaName = type.Assembly.GetName().Name + "/" + schemaResourceName;
            }
            else
            {
                SchemaName = schemaResourceName;
            }

            InitializePropertiesFromDeviceProviderAttribute();
        }
    }
}