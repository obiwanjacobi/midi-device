using System.IO;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.UnitTests
{
    /// <summary>
    ///This is a test class for MidiLogicalXmlWriterTest and is intended
    ///to contain all MidiLogicalXmlWriterTest Unit Tests
    ///</summary>
    
    //[DeploymentItem("LogicalXmlTest/LogicalXmlTestSchema.mds")]
    //[DeploymentItem("LogicalXmlTest/LogicalXmlTestStream.bin")]
    public class LogicalXmlTest
    {
        public static readonly string TestSchemaFileName = "LogicalXmlTestSchema.mds";
        public static readonly string TestStreamFileName = "LogicalXmlTestStream.bin";
        
        private readonly ITestOutputHelper _output;

        public LogicalXmlTest(ITestOutputHelper output)
            => _output = output;

        /// <summary>
        ///A test for Write
        ///</summary>
        [Fact]
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

                _output.WriteLine(logicalWriter.XmlDocument.InnerXml);

                ctx.Reset();

                MidiLogicalXmlReader logicalReader = new MidiLogicalXmlReader(logicalWriter.XmlDocument);

                using (MemoryStream stream = new MemoryStream())
                {
                    ctx.PhysicalStream = stream;
                    ctx.ToPhysical(logicalReader);

                    // rewind streams for comparison
                    stream.Position = 0;
                    physicalStream.Position = 0;

                    Assert.Equal(physicalStream.Length, stream.Length);

                    while (physicalStream.Position < physicalStream.Length)
                    {
                        Assert.Equal(physicalStream.ReadByte(), stream.ReadByte());
                    }
                }
            }
        }
    }
}