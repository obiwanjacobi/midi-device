using System;
using System.IO;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests
{
    /// <summary>
    /// This is a test class for CannedBytes.Midi.Device.MidiDeviceDataManager and is intended
    /// to contain all CannedBytes.Midi.Device.MidiDeviceDataManager Unit Tests
    /// </summary>
    
    [DeploymentItem("TestSchema.mds")]
    public class MidiDeviceDataManagerTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// A test for ConvertorManager
        /// </summary>
        [Fact]
        public void MidiDeviceManagerTest()
        {
            DeviceSchema schema = SchemaHelper.OpenDeviceSchema(
                Path.Combine(TestContext.DeploymentDirectory, Constants.TestSchemaFileName));

            Assert.NotNull(schema);
        }

        /// <summary>
        /// A test for ReadAppend (IMidiLogicalWriter, RecordType, Stream)
        /// </summary>
        [Fact]
        public void ReadAppendTest()
        {
            DeviceSchema schema = SchemaHelper.OpenDeviceSchema(
                Path.Combine(TestContext.DeploymentDirectory, Constants.TestSchemaFileName));

            RecordType recordType = schema.RootRecordTypes[0];

            ConverterManager converterManager = new ConverterManager();
            GroupConverter baseConverter = converterManager.GetConverter(recordType);

            MidiDeviceDataContext ctx = new MidiDeviceDataContext(recordType, baseConverter);

            MidiDeviceDataManager dataMgr = new MidiDeviceDataManager();

            // the logical writer receives the logical midi data
            IMidiLogicalWriter logicalWriter = new MidiLogicalWriterStub();

            using (Stream physicalStream = File.OpenRead(Constants.DeviceSchemaTestStreamFileName))
            {
                long count = dataMgr.ReadAppend(ctx, logicalWriter, physicalStream);

                Assert.True(count > 0);
            }
        }

        /// <summary>
        /// A test for WriteAppend (Stream, RecordType, IMidiLogicalReader)
        /// </summary>
        [Fact]
        public void WriteAppendTest()
        {
            DeviceSchema schema = SchemaHelper.OpenDeviceSchema(
                Path.Combine(TestContext.DeploymentDirectory, Constants.TestSchemaFileName));

            RecordType recordType = schema.RootRecordTypes[0];

            ConverterManager converterManager = new ConverterManager();
            GroupConverter baseConverter = converterManager.GetConverter(recordType);

            MidiDeviceDataContext ctx = new MidiDeviceDataContext(recordType, baseConverter);

            MidiDeviceDataManager dataMgr = new MidiDeviceDataManager();

            // the logical reader fetches logical data
            MidiLogicalReaderStub reader = new MidiLogicalReaderStub();

            // fill the reader stub with logical data
            reader.AddValue<byte>("SysExData", 0, 65);
            reader.AddValue<bool>("Bit0", 0, true);
            reader.AddValue<bool>("Bit1", 0, false);
            reader.AddValue<bool>("Bit2", 0, true);
            reader.AddValue<bool>("Bit3", 0, false);
            reader.AddValue<bool>("Bit4", 0, true);
            reader.AddValue<bool>("Bit5", 0, false);
            reader.AddValue<bool>("Bit6", 0, true);
            reader.AddValue<byte>("LowNibble", 0, 0x0F);
            reader.AddValue<byte>("HiNibble", 0, 0x07);
            reader.AddValue<string>("Name", 0, "midi5");

            using (MemoryStream physicalStream = new MemoryStream())
            {
                long count = dataMgr.WriteAppend(ctx, physicalStream, reader);

                Assert.True(count > 0);

                Console.WriteLine(System.BitConverter.ToString(physicalStream.GetBuffer(), 0, (int)physicalStream.Length));
            }
        }

        
    }
}