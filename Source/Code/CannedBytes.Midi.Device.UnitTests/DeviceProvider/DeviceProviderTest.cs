using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.DeviceProvider
{
    [TestClass]
    [DeploymentItem("DeviceProvider/DeviceTestSchema.mds")]
    public class DeviceProviderTest
    {
        public const string TestSchemaFileName = "DeviceTestSchema.mds";

        public MidiDeviceProvider CreateDeviceProvider()
        {
            var container = DeviceHelper.CreateContainer();
            return container.GetExportedValue<MidiDeviceProvider>();
        }

        [TestMethod]
        public void Init_DeviceSchema_NoErrors()
        {
            var manufacturer = "UnitTest";
            var model = "Model-1";
            byte manId = 0x10;
            byte modId = 0x12;

            var provider = CreateDeviceProvider();
            provider.Initialze(TestSchemaFileName, manufacturer, model, manId, modId);

            Assert.AreEqual(manufacturer, provider.Manufacturer);
            Assert.AreEqual(model, provider.ModelName);
            Assert.AreEqual(manId, provider.ManufacturerId);
            Assert.AreEqual(modId, provider.ModelId);

            Assert.IsNotNull(provider.Schema);
            Assert.AreEqual(provider.Schema.RootRecordTypes.Count, provider.RootTypes.Count);
        }
    }
}