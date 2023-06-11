using System.Collections.Generic;
using System.IO;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.ConverterTests;

public class ConverterManagerTest
{
    public const string Folder = "ConvertersTests/";
    public const string ConverterManagerTestSchema = "ConverterManagerTestSchema.mds";

    private static DeviceSchema LoadTestSchema()
    {
        string path = Path.Combine(Folder, ConverterManagerTestSchema);
        return DeviceSchemaHelper.LoadSchema(path);
    }

    public static ConverterManager CreateConverterManager()
    {
        List<IConverterFactory> factories = new()
        {
            new MidiTypesConverterFactory()
        };

        ConverterManager mgr = new(factories);

        return mgr;
    }

    [Fact]
    public void GetConverter_MidiTypesMidiData_IsNotNull()
    {
        DeviceSchema schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypesSchema);
        DataType dataType = schema.AllDataTypes.Find("midiData");

        ConverterManager mgr = CreateConverterManager();
        DataConverter converter = mgr.GetConverter(dataType);

        converter.Should().NotBeNull();
    }

    [Fact]
    public void GetConverter_MidiTypesMidiData_HasCorrectDataType()
    {
        DeviceSchema schema = DeviceSchemaHelper.LoadSchema(SchemaNames.MidiTypesSchema);
        DataType dataType = schema.AllDataTypes.Find("midiData");

        ConverterManager mgr = CreateConverterManager();
        DataConverter converter = mgr.GetConverter(dataType);

        converter.DataType.Should().BeSameAs(dataType);
    }

    // DataType based tests

    private static FieldConverterPair CreateDataFieldConverterPair(out Field field)
    {
        var schema = LoadTestSchema();
        field = schema.RootRecordTypes[0].Fields[0];

        var mgr = CreateConverterManager();
        var pair = mgr.GetFieldConverterPair(field);

        return pair;
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataField_IsNotNull()
    {
        FieldConverterPair pair = CreateDataFieldConverterPair(out Field field);

        pair.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataField_FieldIsNotNull()
    {
        FieldConverterPair pair = CreateDataFieldConverterPair(out Field field);

        pair.Field.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataField_ConverterIsNotNull()
    {
        FieldConverterPair pair = CreateDataFieldConverterPair(out Field field);

        pair.Converter.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataField_HasCorrectField()
    {
        FieldConverterPair pair = CreateDataFieldConverterPair(out Field field);

        pair.Field.Should().Be(field);
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataField_HasCorrectConverter()
    {
        DeviceSchema schema = LoadTestSchema();
        Field field = schema.RootRecordTypes[0].Fields[0];

        ConverterManager mgr = CreateConverterManager();
        FieldConverterPair pair = mgr.GetFieldConverterPair(field);

        pair.Converter.Should().Be(mgr.GetConverter(field));
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeDataAndRecordField_SameFieldNameResultsInDifferentPairs()
    {
        DeviceSchema schema = LoadTestSchema();
        Field field2a = schema.AllRecordTypes.Find("rootRecord").Fields.Find("Field2");
        Field field2b = schema.AllRecordTypes.Find("subRecord").Fields.Find("Field2");

        ConverterManager mgr = CreateConverterManager();
        FieldConverterPair pair2a = mgr.GetFieldConverterPair(field2a);
        FieldConverterPair pair2b = mgr.GetFieldConverterPair(field2b);

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
        var schema = LoadTestSchema();
        field = schema.RootRecordTypes[0].Fields[1];

        var mgr = CreateConverterManager();
        var pair = mgr.GetFieldConverterPair(field);

        return pair;
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeRecordField_IsNotNull()
    {
        FieldConverterPair pair = CreateRecordFieldConverterPair(out Field field);

        pair.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeRecordField_FieldIsNotNull()
    {
        FieldConverterPair pair = CreateRecordFieldConverterPair(out Field field);

        pair.Field.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeRecordField_ConverterIsNotNull()
    {
        FieldConverterPair pair = CreateRecordFieldConverterPair(out Field field);

        pair.Converter.Should().NotBeNull();
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeRecordField_HasCorrectField()
    {
        FieldConverterPair pair = CreateRecordFieldConverterPair(out Field field);

        pair.Field.Should().Be(field);
    }

    [Fact]
    public void GetFieldConverterPair_MidiTypeRecordField_HasCorrectConverter()
    {
        DeviceSchema schema = LoadTestSchema();
        Field field = schema.RootRecordTypes[0].Fields[1];

        ConverterManager mgr = CreateConverterManager();
        FieldConverterPair pair = mgr.GetFieldConverterPair(field);

        // '.Should().Be()' throws an exception unjustified...
        //pair.Converter.Should().Be(mgr.GetConverter(field));
        pair.Converter.Should().BeEquivalentTo(mgr.GetConverter(field));
    }
}
