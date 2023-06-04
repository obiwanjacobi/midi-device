using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.Schema.Xml;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests
{
    public static class DeviceHelper
    {
        public static DeviceSchema OpenDeviceSchema(string fileName)
        {
            var schemaProvider = new MidiDeviceSchemaProvider();

            var schema = schemaProvider.Load(fileName);

            return schema;
        }

        public static MidiDeviceDataContext ReadLogical(string schemaFile, string testStreamFile, string recordTypeName, IMidiLogicalWriter logicalWriter)
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(schemaFile);
            Assert.NotNull(schema);

            RecordType recordType = schema.RootRecordTypes.Find(recordTypeName);
            Assert.NotNull(recordType);

            var container = CreateContainer();
            ConverterManager converterManager = new ConverterManager();
            converterManager.InitializeFrom(container);

            GroupConverter baseConverter = converterManager.GetConverter(recordType);
            Assert.NotNull(baseConverter);

            MidiDeviceDataContext ctx = new MidiDeviceDataContext(recordType, baseConverter);
            ctx.CompositionContainer = container;

            if (logicalWriter == null)
            {
                logicalWriter = new ConsoleLogicalWriterStub();
            }
            else
            {
                logicalWriter = new ConsoleLogicalWriterStub(logicalWriter);
            }

            using (Stream physicalStream = File.OpenRead(testStreamFile))
            {
                ctx.PhysicalStream = physicalStream;
                ctx.ToLogical(logicalWriter);
            }

            return ctx;
        }

        public static MidiDeviceDataContext WritePhysical(string schemaFile, string recordTypeName, IMidiLogicalReader logicalReader)
        {
            DeviceSchema schema = DeviceHelper.OpenDeviceSchema(schemaFile);
            Assert.NotNull(schema);

            RecordType recordType = schema.RootRecordTypes.Find(recordTypeName);
            Assert.NotNull(recordType);

            var container = CreateContainer();
            ConverterManager converterManager = new ConverterManager();
            converterManager.InitializeFrom(container);

            GroupConverter baseConverter = converterManager.GetConverter(recordType);
            Assert.NotNull(baseConverter);

            MidiDeviceDataContext ctx = new MidiDeviceDataContext(recordType, baseConverter);
            ctx.CompositionContainer = container;

            MemoryStream physicalStream = new MemoryStream();
            {
                ctx.PhysicalStream = physicalStream;
                ctx.ToPhysical(logicalReader);

                Console.WriteLine(System.BitConverter.ToString(physicalStream.GetBuffer(), 0, (int)physicalStream.Length));
            }

            return ctx;
        }

        public static CompositionContainer CreateContainer()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));

            var container = new CompositionContainer(catalog);
            return container;
        }

        public static bool CompareStreams(Stream stream1, Stream stream2, out long position)
        {
            position = -1;

            var repos1 = stream1.Position;
            var repos2 = stream2.Position;

            stream1.Position = 0;
            stream2.Position = 0;

            // ignore checksum
            var count = stream1.Length - stream1.Position - 2;
            for (long i = 0; i < count && position < 0; i++)
            {
                var val1 = stream1.ReadByte();
                var val2 = stream2.ReadByte();

                if (val1 != val2)
                {
                    position = i;
                }
            }

            stream1.Position = repos1;
            stream2.Position = repos2;

            return position < 0 && stream1.Length == stream2.Length;
        }

        public static string CompareLogicalData(
            DictionaryBasedLogicalStub logicalData1, DictionaryBasedLogicalStub logicalData2)
        {
            foreach (var pair in logicalData1.FieldValues)
            {
                if (!logicalData2.FieldValues.ContainsKey(pair.Key))
                {
                    Console.WriteLine("CompareLogicalData could not find '" + pair.Key + "'.");
                    return pair.Key;
                }

                var value1 = (IComparable)pair.Value;
                var value2 = (IComparable)logicalData2.FieldValues[pair.Key];

                if (value1.CompareTo(value2) != 0)
                {
                    Console.WriteLine("CompareLogicalData values do not match '" + pair.Value + "' - '" + logicalData2.FieldValues[pair.Key] + "'.");
                    return pair.Key;
                }
            }

            return null;
        }

        public static string StreamToString(Stream stream)
        {
            MemoryStream writeStream = stream as MemoryStream;
            long repos = stream.Position;
            stream.Position = 0;

            if (writeStream == null)
            {
                writeStream = new MemoryStream();
                stream.CopyTo(writeStream);
            }

            var text = System.BitConverter.ToString(writeStream.GetBuffer(), 0, (int)writeStream.Length);

            stream.Position = repos;

            return text.Replace('-', ' ');
        }
    }
}