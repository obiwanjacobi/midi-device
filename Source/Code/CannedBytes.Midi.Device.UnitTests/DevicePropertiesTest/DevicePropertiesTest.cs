using CannedBytes.Midi.Device.UnitTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests.DevicePropertiesTest
{
    [TestClass]
    [DeploymentItem("DevicePropertiesTest/DevicePropertiesTestSchema.mds")]
    [DeploymentItem("DevicePropertiesTest/DevicePropertiesTestStream.bin")]
    public class DevicePropertiesTest
    {
        public const string SchemaFileName = "DevicePropertiesTestSchema.mds";
        public const string StreamFileName = "DevicePropertiesTestStream.bin";

        [TestMethod]
        public void ToLogical_SchemaWithProperties_LogicalPropertyValuesInContext()
        {
            var writer = new DictionaryBasedLogicalStub();

            var ctx = DeviceHelper.ReadLogical(SchemaFileName, StreamFileName, "RQ1", writer);

            // 2 fields are fixed value - logical not called
            Assert.AreEqual(2, ctx.DeviceProperties.Count);
            Assert.AreEqual(65, ctx.DeviceProperties.Find("ManufacturerId").GetValue<int>());
            Assert.AreEqual(16, ctx.DeviceProperties.Find("SysExChannel").GetValue<int>());
            //Assert.AreEqual(43, ctx.DeviceProperties.Find("ModelId").GetValue<int>());
            //Assert.AreEqual(17, ctx.DeviceProperties.Find("CommandId").GetValue<int>());
        }
    }
}