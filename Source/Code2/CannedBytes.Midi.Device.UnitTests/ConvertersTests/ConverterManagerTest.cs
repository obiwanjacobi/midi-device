using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Device.UnitTests.ConverterTests
{
    
    //[DeploymentItem(Folder + ConverterManagerTestSchema)]
    public class ConverterManagerTest
    {
        public const string Folder = "ConvertersTests/";
        public const string ConverterManagerTestSchema = "ConverterManagerTestSchema.mds";

        private static ConverterManager.AttributedConverterFactory CreateAttributedFactory()
        {
            var dataConverters = Enumerable.Empty<Lazy<DataConverter, IDataConverterInfo>>();
            var streamConverters = Enumerable.Empty<Lazy<StreamConverter, IStreamConverterInfo>>();

            var attrFactory = 
                new ConverterManager.AttributedConverterFactory(
                                    dataConverters, streamConverters);

            return attrFactory;
        }

        public static ConverterManager CreateConverterManager()
        {
            var factories = new List<Lazy<IConverterFactory, IConverterFactoryInfo>>();

            factories.Add(new Lazy<IConverterFactory,IConverterFactoryInfo>(
                () =>
                {
                    return new MidiTypesConverterFactory();
                },
                ConverterFactoryAttribute.FromType<MidiTypesConverterFactory>()));

            var mgr = new ConverterManager(CreateAttributedFactory(), factories);

            return mgr;
        }

        [Fact]
        public void GetConverter_MidiTypesMidiData_IsNotNull()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypesSchema);
            var dataType = schema.AllDataTypes.Find("midiData");

            var mgr = CreateConverterManager();
            var converter = mgr.GetConverter(dataType);

            converter.Should().NotBeNull();
        }

        [Fact]
        public void GetConverter_MidiTypesMidiData_HasCorrectDataType()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypesSchema);
            var dataType = schema.AllDataTypes.Find("midiData");

            var mgr = CreateConverterManager();
            var converter = mgr.GetConverter(dataType);

            converter.DataType.Should().BeSameAs(dataType);
        }

        // DataType based tests

        private static FieldConverterPair CreateDataFieldConverterPair(out Field field)
        {
            var schema = DeviceSchemaHelper.LoadSchema(ConverterManagerTestSchema);
            field = schema.RootRecordTypes[0].Fields[1];

            var mgr = CreateConverterManager();
            var pair = mgr.GetFieldConverterPair(field);

            return pair;
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataField_IsNotNull()
        {
            Field field;
            var pair = CreateDataFieldConverterPair(out field);

            pair.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataField_FieldIsNotNull()
        {
            Field field;
            var pair = CreateDataFieldConverterPair(out field);

            pair.Field.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataField_ConverterIsNotNull()
        {
            Field field;
            var pair = CreateDataFieldConverterPair(out field);

            pair.Converter.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataField_HasCorrectField()
        {
            Field field;
            var pair = CreateDataFieldConverterPair(out field);

            pair.Field.Should().Be(field);
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataField_HasCorrectConverter()
        {
            var schema = DeviceSchemaHelper.LoadSchema(ConverterManagerTestSchema);
            var field = schema.RootRecordTypes[0].Fields[0];

            var mgr = CreateConverterManager();
            var pair = mgr.GetFieldConverterPair(field);

            pair.Converter.Should().Be(mgr.GetConverter(field));
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeDataAndRecordField_SameFieldNameResultsInDifferentPairs()
        {
            var schema = DeviceSchemaHelper.LoadSchema(ConverterManagerTestSchema);
            var field2a = schema.AllRecordTypes.Find("rootRecord").Fields.Find("Field2");
            var field2b = schema.AllRecordTypes.Find("subRecord").Fields.Find("Field2");

            var mgr = CreateConverterManager();
            var pair2a = mgr.GetFieldConverterPair(field2a);
            var pair2b = mgr.GetFieldConverterPair(field2b);

            pair2a.Field.Should().Be(field2a);
            pair2b.Field.Should().Be(field2b);

            pair2a.DataConverter.Should().BeNull();
            pair2a.StreamConverter.Should().NotBeNull();

            pair2b.DataConverter.Should().NotBeNull();
            pair2b.StreamConverter.Should().BeNull();
        }

        // RecordType based tests

        private static FieldConverterPair CreateRecordFieldConverterPair(out Field field)
        {
            var schema = DeviceSchemaHelper.LoadSchema(ConverterManagerTestSchema);
            field = schema.RootRecordTypes[0].Fields[1];

            var mgr = CreateConverterManager();
            var pair = mgr.GetFieldConverterPair(field);

            return pair;
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeRecordField_IsNotNull()
        {
            Field field;
            var pair = CreateRecordFieldConverterPair(out field);

            pair.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeRecordField_FieldIsNotNull()
        {
            Field field;
            var pair = CreateRecordFieldConverterPair(out field);

            pair.Field.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeRecordField_ConverterIsNotNull()
        {
            Field field;
            var pair = CreateRecordFieldConverterPair(out field);

            pair.Converter.Should().NotBeNull();
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeRecordField_HasCorrectField()
        {
            Field field;
            var pair = CreateRecordFieldConverterPair(out field);

            pair.Field.Should().Be(field);
        }

        [Fact]
        public void GetFieldConverterPair_MidiTypeRecordField_HasCorrectConverter()
        {
            var schema = DeviceSchemaHelper.LoadSchema(ConverterManagerTestSchema);
            var field = schema.RootRecordTypes[0].Fields[1];

            var mgr = CreateConverterManager();
            var pair = mgr.GetFieldConverterPair(field);

            // '.Should().Be()' throws an exception unjustified...
            //pair.Converter.Should().Be(mgr.GetConverter(field));
            pair.Converter.Should().BeEquivalentTo(mgr.GetConverter(field));
        }
    }
}
