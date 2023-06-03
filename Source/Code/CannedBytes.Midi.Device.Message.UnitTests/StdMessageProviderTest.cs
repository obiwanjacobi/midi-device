using System.IO;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    /// <summary>
    /// Summary description for StdMessageProviderTest
    /// </summary>
    [TestClass]
    [DeploymentItem("TestMessageProvider.xsd")]
    public class StdMessageProviderTest
    {
        public static readonly string TestMessageProviderFileName = "TestMessageProvider.xsd";

        //[TestMethod]
        public void TestGetMessageInfo_EnvelopeMessage()
        {
            IMessageProvider msgProvider = CreateMessageProvider();

            using (Stream stream = new MemoryStream(new byte[] { 0xF0, 10, 21, 11, 12 }))
            {
                MidiDeviceMessageInfo msgInfo = msgProvider.GetMessageInfo(stream);

                Assert.IsNotNull(msgInfo, "No MidiDeviceMessageInfo was returned");
                Assert.IsNotNull(msgInfo.EnvelopeRecordType, "No Envelope RecordType was set.");
                Assert.AreEqual("EnvelopeMessage", msgInfo.EnvelopeRecordType.Name.Name, "Expected RecordType was not returned.");
            }
        }

        //[TestMethod]
        public void TestGetMessageInfo_SomeOtherMessage()
        {
            IMessageProvider msgProvider = CreateMessageProvider();

            using (Stream stream = new MemoryStream(new byte[] { 0xF0, 10, 21, 11, 22 }))
            {
                MidiDeviceMessageInfo msgInfo = msgProvider.GetMessageInfo(stream);

                Assert.IsNotNull(msgInfo, "No MidiDeviceMessageInfo was returned");
                Assert.IsNotNull(msgInfo.EnvelopeRecordType, "No Envelope RecordType was set.");
                Assert.AreEqual("SomeOtherMessage", msgInfo.EnvelopeRecordType.Name.Name, "Expected RecordType was not returned.");
            }
        }

        internal static IMessageProvider CreateMessageProvider()
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(TestMessageProviderFileName);

            return new StdMessageProvider(schema);
        }
    }
}