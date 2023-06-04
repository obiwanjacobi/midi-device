using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests;
using CannedBytes.Midi.Device.UnitTests.Stubs;
using Xunit;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Message.UnitTests
{
    public class MessageTester
    {
        public bool EnableTrace { get; set; }

        public CompositionContainer CompositionContainer { get; set; }

        public MessageDeviceProvider DeviceProvider { get; set; }

        public FieldConverterPair CurrentPair { get; private set; }

        public MidiDeviceBinaryMap CurrentBinaryMap { get; private set; }

        public void SetCurrentMessage(string messageName)
        {
            Assert.NotNull(DeviceProvider);

            CurrentPair = DeviceProvider.RootTypes.Find(DeviceProvider.Schema.SchemaName + ":" + messageName);
            Assert.NotNull(CurrentPair);

            Log("Current FieldConverterPair: " + CurrentPair.Field.ToString());

            CurrentBinaryMap = DeviceProvider.FindBinaryMap(CurrentPair);
            Assert.NotNull(CurrentBinaryMap);

            //Log(CurrentBinaryMap.ToString());
        }

        private void Log(string text)
        {
            if (EnableTrace)
            {
                Console.WriteLine(text);
            }
        }

        public MidiMessageDataContext ReadLogicalData(Stream physicalStream, IMidiLogicalWriter writer)
        {
            Assert.NotNull(physicalStream);
            Assert.NotNull(writer);
            Assert.NotNull(CompositionContainer);
            Assert.NotNull(DeviceProvider);
            Assert.NotNull(CurrentPair);
            Assert.NotNull(CurrentBinaryMap);

            var ctx = new MidiMessageDataContext(CurrentPair);
            ctx.CompositionContainer = CompositionContainer;
            ctx.BinaryMap = CurrentBinaryMap;
            ctx.PhysicalStream = physicalStream;

            if (EnableTrace &&
                !(writer is ConsoleLogicalWriterStub))
            {
                writer = new ConsoleLogicalWriterStub(writer);
            }

            try
            {
                ctx.ToLogical(writer);
            }
            finally
            {
                if (EnableTrace)
                {
                    Console.WriteLine("Reading Data Records:");
                    Console.WriteLine(ctx.DataRecords.ToString());
                }
            }

            return ctx;
        }

        public MidiMessageDataContext WritePhysicalData(IMidiLogicalReader reader, Stream outputStream)
        {
            Assert.NotNull(reader);
            Assert.NotNull(outputStream);
            Assert.NotNull(CompositionContainer);
            Assert.NotNull(DeviceProvider);
            Assert.NotNull(CurrentPair);
            Assert.NotNull(CurrentBinaryMap);

            var ctx = new MidiMessageDataContext(CurrentPair);
            ctx.CompositionContainer = CompositionContainer;
            ctx.BinaryMap = CurrentBinaryMap;
            ctx.PhysicalStream = outputStream;

            try
            {
                ctx.ToPhysical(reader);
            }
            finally
            {
                if (EnableTrace)
                {
                    Console.WriteLine("Writing Data Records:");
                    Console.WriteLine(ctx.DataRecords.ToString());
                }
            }

            return ctx;
        }

        public bool Lookup(Field firstField, FieldPathKey firstKey, Field lastField, FieldPathKey lastKey,
            out SevenBitUInt32 address, out SevenBitUInt32 size)
        {
            Assert.NotNull(firstField);
            Assert.NotNull(firstKey);
            Assert.NotNull(lastField);
            Assert.NotNull(lastKey);
            Assert.NotNull(CompositionContainer);
            Assert.NotNull(CurrentBinaryMap);

            var converterMgr = CompositionContainer.GetExportedValue<ConverterManager>();
            var schemaProvider = CompositionContainer.GetExportedValue<IDeviceSchemaProvider>();
            var factory = new MessageTypeFactory(converterMgr, CurrentBinaryMap, schemaProvider);

            return factory.FindAddressRange(firstField, firstKey, lastField, lastKey, out address, out size);
        }

        public bool Lookup(DictionaryBasedLogicalStub logicalData, out SevenBitUInt32 address, out SevenBitUInt32 size)
        {
            // check if there is an address in the logical data
            var addressData = (from field in logicalData.Fields
                               where field.Field.DevicePropertyName == "address"
                               from pair in logicalData.FieldValues
                               where pair.Key == field.Field.Name.FullName + "[" + field.Key + "]"
                               select pair.Value).FirstOrDefault();

            DictionaryBasedLogicalStub.FieldInfo startField = null;

            // retrieve first and last field
            if (addressData != null)
            {
                address = SevenBitUInt32.FromSevenBitValue((uint)Convert.ChangeType(addressData, typeof(uint)));
                var startNode = CurrentBinaryMap.FindFirst(address);

                if (startNode != null)
                {
                    var endNode = (from field in logicalData.Fields.Reverse()
                                   let currentNode = startNode.Find(field.Field, field.Key, (node) => { return node.NextField; })
                                   where currentNode != null
                                   where currentNode.IsAddressMap
                                   select currentNode).FirstOrDefault();

                    var converterMgr = CompositionContainer.GetExportedValue<ConverterManager>();
                    var schemaProvider = CompositionContainer.GetExportedValue<IDeviceSchemaProvider>();
                    var factory = new MessageTypeFactory(converterMgr, CurrentBinaryMap, schemaProvider);

                    size = factory.GetAddressSize(startNode, endNode);
                    return true;
                }
            }

            if (startField == null)
            {
                startField = (from field in logicalData.Fields
                              let node = CurrentBinaryMap.Find(field.Field, field.Key)
                              where node != null
                              where node.IsAddressMap
                              select field).FirstOrDefault();
            }

            var lastField = (from field in logicalData.Fields.Reverse()
                             let node = CurrentBinaryMap.Find(field.Field, field.Key)
                             where node != null
                             where node.IsAddressMap
                             select field).FirstOrDefault();

            Assert.NotNull(startField);
            Assert.NotNull(lastField);

            return Lookup(startField.Field, startField.Key, lastField.Field, lastField.Key, out address, out size);
        }

        public MidiMessageDataContext WritePhysicalData(IMidiLogicalReader reader,
            SevenBitUInt32 address, SevenBitUInt32 size, Stream outputStream)
        {
            Assert.NotNull(reader);
            Assert.NotNull(outputStream);
            Assert.NotNull(CompositionContainer);
            Assert.NotNull(DeviceProvider);

            var ctx = new MidiMessageDataContext(CurrentPair);
            ctx.CompositionContainer = CompositionContainer;
            ctx.BinaryMap = CurrentBinaryMap;
            ctx.PhysicalStream = outputStream;

            // prepare address map
            ctx.DeviceProperties.Add(Constants.MidiTypesNamespace, "address", address);
            ctx.DeviceProperties.Add(Constants.MidiTypesNamespace, "size", size);

            try
            {
                ctx.ToPhysical(reader);
            }
            finally
            {
                if (EnableTrace)
                {
                    Console.WriteLine("Writing Data Records:");
                    Console.WriteLine(ctx.DataRecords.ToString());
                }
            }

            return ctx;
        }

        public void ReadWriteComparePhysicalData(string streamFileName)
        {
            var logicalData = new DictionaryBasedLogicalStub();

            using (var fileStream = File.OpenRead(streamFileName))
            {
                ReadLogicalData(fileStream, logicalData);

                SevenBitUInt32 address;
                SevenBitUInt32 size;

                if (Lookup(logicalData, out address, out size))
                {
                    using (var memStream = new MemoryStream())
                    {
                        WritePhysicalData(logicalData, address, size, memStream);

                        if (EnableTrace)
                        {
                            Console.WriteLine("S: " + DeviceHelper.StreamToString(fileStream));
                            Console.WriteLine("M: " + DeviceHelper.StreamToString(memStream));
                        }

                        long pos = 0;
                        if (!DeviceHelper.CompareStreams(fileStream, memStream, out pos))
                        {
                            Assert.Fail("The resulting stream differs from the physical stream at position " + pos);
                        }
                    }
                }
                else
                {
                    Assert.Fail("Could not find the address (and size).");
                }
            }
        }

        public void ReadWriteReadCompareLogicalData(string streamFileName)
        {
            using (var fileStream = File.OpenRead(streamFileName))
            {
                ReadWriteReadCompareLogicalData(fileStream);
            }
        }

        public void ReadWriteReadCompareLogicalData(Stream sysExStream)
        {
            var logicalData = new DictionaryBasedLogicalStub();

            ReadLogicalData(sysExStream, logicalData);

            SevenBitUInt32 address;
            SevenBitUInt32 size;

            if (Lookup(logicalData, out address, out size))
            {
                using (var memStream = new MemoryStream())
                {
                    var writeCtx = WritePhysicalData(logicalData, address, size, memStream);

                    if (EnableTrace)
                    {
                        Console.WriteLine("S: " + DeviceHelper.StreamToString(writeCtx.PhysicalStream));
                        Console.WriteLine("M: " + DeviceHelper.StreamToString(sysExStream));
                    }

                    Assert.Equal(writeCtx.PhysicalStream.Length, memStream.Length);

                    long pos = 0;

                    if (!DeviceHelper.CompareStreams(writeCtx.PhysicalStream, sysExStream, out pos))
                    {
                        if (!EnableTrace)
                        {
                            Console.WriteLine("S: " + DeviceHelper.StreamToString(writeCtx.PhysicalStream));
                            Console.WriteLine("M: " + DeviceHelper.StreamToString(sysExStream));
                        }
                        Console.WriteLine("WARNING: The resulting stream differs from the physical stream at position " + pos);
                    }

                    memStream.Position = 0;

                    var logicalWriter = new DictionaryBasedLogicalStub();
                    var readBackCtx = ReadLogicalData(memStream, logicalWriter);

                    var fieldInfo = DeviceHelper.CompareLogicalData(logicalData, logicalWriter);

                    Assert.Null(fieldInfo);
                }
            }
            else
            {
                Assert.Fail("Could not find the address (and size).");
            }
        }

        public void ReadWriteReadAllCompareLogicalData(string streamFileName)
        {
            using (var fileStream = File.OpenRead(streamFileName))
            {
                var multiStream = new MultiSysExStream(fileStream);
                int errorCount = 0;
                int bufferCount = 0;

                while (multiStream.MoveNext())
                {
                    bufferCount++;
                    var pos = multiStream.Position;

                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("------------------------------------------------------------------------------------");
                        Console.WriteLine("Info: ReadWriteReadAllComapareLogicalData start reading buffer " + bufferCount);

                        ReadWriteReadCompareLogicalData(multiStream);

                        Console.WriteLine("Info: ReadWriteReadAllComapareLogicalData success for buffer starting at position " + pos);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: ReadWriteReadAllComapareLogicalData failed for buffer starting at position " + pos);
                        Console.WriteLine(e.ToString());

                        errorCount++;
                    }
                }

                Assert.Equal(0, errorCount);
            }
        }
    }
}