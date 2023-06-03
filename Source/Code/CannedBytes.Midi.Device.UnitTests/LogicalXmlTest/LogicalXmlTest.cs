using System.IO;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests
{
    /// <summary>
    ///This is a test class for MidiLogicalXmlWriterTest and is intended
    ///to contain all MidiLogicalXmlWriterTest Unit Tests
    ///</summary>
    [TestClass()]
    [DeploymentItem("LogicalXmlTest/LogicalXmlTestSchema.mds")]
    [DeploymentItem("LogicalXmlTest/LogicalXmlTestStream.bin")]
    public class LogicalXmlTest
    {
        public static readonly string TestSchemaFileName = "LogicalXmlTestSchema.mds";
        public static readonly string TestStreamFileName = "LogicalXmlTestStream.bin";

        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for Write
        ///</summary>
        [TestMethod()]
        public void WriteXmlTest()
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(TestSchemaFileName);

            RecordType recordType = schema.RootRecordTypes[0];

            var container = DeviceHelper.CreateContainer();
            ConverterManager converterManager = new ConverterManager();
            converterManager.InitializeFrom(container);

            GroupConverter baseConverter = converterManager.GetConverter(recordType);

            MidiDeviceDataContext ctx = new MidiDeviceDataContext(recordType, baseConverter);
            ctx.CompositionContainer = container;

            // the logical writer receives the logical midi data
            MidiLogicalXmlWriter logicalWriter = new MidiLogicalXmlWriter();

            using (Stream physicalStream = File.OpenRead(TestStreamFileName))
            {
                ctx.PhysicalStream = physicalStream;
                ctx.ToLogical(logicalWriter);

                this.TestContext.WriteLine(logicalWriter.XmlDocument.InnerXml);

                ctx.Reset();

                MidiLogicalXmlReader logicalReader = new MidiLogicalXmlReader(logicalWriter.XmlDocument);

                using (MemoryStream stream = new MemoryStream())
                {
                    ctx.PhysicalStream = stream;
                    ctx.ToPhysical(logicalReader);

                    // rewind streams for comparison
                    stream.Position = 0;
                    physicalStream.Position = 0;

                    Assert.AreEqual(physicalStream.Length, stream.Length);

                    while (physicalStream.Position < physicalStream.Length)
                    {
                        Assert.AreEqual(physicalStream.ReadByte(), stream.ReadByte(), "Stream not equal at position " + physicalStream.Position);
                    }
                }
            }
        }
    }
}