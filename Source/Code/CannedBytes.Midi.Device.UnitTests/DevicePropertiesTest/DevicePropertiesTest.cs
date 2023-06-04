using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.DevicePropertiesTest
{
    
    //[DeploymentItem("DevicePropertiesTest/DevicePropertiesTestSchema.mds")]
    //[DeploymentItem("DevicePropertiesTest/DevicePropertiesTestStream.bin")]
    public class DevicePropertiesTest
    {
        public const string SchemaFileName = "DevicePropertiesTestSchema.mds";
        public const string StreamFileName = "DevicePropertiesTestStream.bin";

        [Fact]
        public void ToLogical_SchemaWithProperties_LogicalPropertyValuesInContext()
        {
            var writer = new DictionaryBasedLogicalStub();

            var ctx = DeviceHelper.ReadLogical(SchemaFileName, StreamFileName, "RQ1", writer);

            // 2 fields are fixed value - logical not called
            Assert.Equal(2, ctx.DeviceProperties.Count);
            Assert.Equal(65, ctx.DeviceProperties.Find("ManufacturerId").GetValue<int>());
            Assert.Equal(16, ctx.DeviceProperties.Find("SysExChannel").GetValue<int>());
            //Assert.Equal(43, ctx.DeviceProperties.Find("ModelId").GetValue<int>());
            //Assert.Equal(17, ctx.DeviceProperties.Find("CommandId").GetValue<int>());
        }
    }
}