using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.DeviceProvider
{
    
    //[DeploymentItem("DeviceProvider/DeviceTestSchema.mds")]
    public class DeviceProviderTest
    {
        public const string TestSchemaFileName = "DeviceTestSchema.mds";

        public MidiDeviceProvider CreateDeviceProvider()
        {
            var container = DeviceHelper.CreateContainer();
            return container.GetExportedValue<MidiDeviceProvider>();
        }

        [Fact]
        public void Init_DeviceSchema_NoErrors()
        {
            var manufacturer = "UnitTest";
            var model = "Model-1";
            byte manId = 0x10;
            byte modId = 0x12;

            var provider = CreateDeviceProvider();
            provider.Initialze(TestSchemaFileName, manufacturer, model, manId, modId);

            Assert.Equal(manufacturer, provider.Manufacturer);
            Assert.Equal(model, provider.ModelName);
            Assert.Equal(manId, provider.ManufacturerId);
            Assert.Equal(modId, provider.ModelId);

            Assert.NotNull(provider.Schema);
            Assert.Equal(provider.Schema.RootRecordTypes.Count, provider.RootTypes.Count);
        }
    }
}