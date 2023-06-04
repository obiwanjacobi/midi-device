using System.IO;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Xunit;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    /// <summary>
    /// Summary description for StdMessageProviderTest
    /// </summary>
    
    //[DeploymentItem("TestMessageProvider.xsd")]
    public class StdMessageProviderTest
    {
        public static readonly string TestMessageProviderFileName = "TestMessageProvider.xsd";

        //[Fact]
        public void TestGetMessageInfo_EnvelopeMessage()
        {
            IMessageProvider msgProvider = CreateMessageProvider();

            using (Stream stream = new MemoryStream(new byte[] { 0xF0, 10, 21, 11, 12 }))
            {
                MidiDeviceMessageInfo msgInfo = msgProvider.GetMessageInfo(stream);

                Assert.NotNull(msgInfo);
                Assert.NotNull(msgInfo.EnvelopeRecordType);
                Assert.Equal("EnvelopeMessage", msgInfo.EnvelopeRecordType.Name.Name);
            }
        }

        //[Fact]
        public void TestGetMessageInfo_SomeOtherMessage()
        {
            IMessageProvider msgProvider = CreateMessageProvider();

            using (Stream stream = new MemoryStream(new byte[] { 0xF0, 10, 21, 11, 22 }))
            {
                MidiDeviceMessageInfo msgInfo = msgProvider.GetMessageInfo(stream);

                Assert.NotNull(msgInfo);
                Assert.NotNull(msgInfo.EnvelopeRecordType);
                Assert.Equal("SomeOtherMessage", msgInfo.EnvelopeRecordType.Name.Name);
            }
        }

        internal static IMessageProvider CreateMessageProvider()
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(TestMessageProviderFileName);

            return new StdMessageProvider(schema);
        }
    }
}
