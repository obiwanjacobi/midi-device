using System.Collections.Generic;
using System.IO;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests;

public class FieldHierarchicalIteratorTest
{
    public const string Folder = "SchemaTests/";
    public const string HierarchicalSchema = "HierarchicalSchema.mds";

    private static DeviceSchema LoadTestSchema()
    {
        string path = Path.Combine(Folder, HierarchicalSchema);
        return DeviceSchemaHelper.LoadSchema(path);
    }

    public static int EnumerateFields(IEnumerable<FieldInfo> iterator)
    {
        int counter = 0;

        foreach (FieldInfo fieldInfo in iterator)
        {
            if (counter % 2 == 0)
            {
                fieldInfo.Field.Name.Name.Should().EndWith("Field1");
            }
            else
            {
                fieldInfo.Field.Name.Name.Should().EndWith("Field2");
            }

            counter++;
        }

        return counter;
    }

    [Fact]
    public void CheckLoadSchema_HierarchicalSchema_LoadsWithoutErrors()
    {
        DeviceSchema schema = LoadTestSchema();

        schema.Should().NotBeNull();
        schema.RootRecordTypes.Should().NotBeEmpty();
    }

    [Fact]
    public void MoveNext_HierarchicalSchema_EnumeratorLoopsOnce()
    {
        DeviceSchema schema = LoadTestSchema();
        FieldHierarchicalIterator iterator = new(schema.RootRecordTypes[0]);

        IEnumerator<FieldInfo> enumerator = iterator.GetEnumerator();

        enumerator.Should().NotBeNull();
        enumerator.MoveNext().Should().BeTrue();
        enumerator.Current.Should().NotBeNull();
    }

    [Fact]
    public void MoveNext_HierarchicalSchema_ReturnsFields()
    {
        DeviceSchema schema = LoadTestSchema();
        FieldHierarchicalIterator iterator = new(schema.RootRecordTypes[0]);

        foreach (FieldInfo field in iterator)
        {
            field.Should().NotBeNull();
        }
    }

    [Fact]
    public void MoveNext_HierarchicalSchema_VerifyAllFields()
    {
        DeviceSchema schema = LoadTestSchema();
        FieldHierarchicalIterator iterator = new(schema.RootRecordTypes[0]);

        int counter = EnumerateFields(iterator);

        counter.Should().Be(6);
    }



    [Fact]
    public void MoveNext_HierarchicalSchema_VerifyAllRepeatedFields()
    {
        DeviceSchema schema = LoadTestSchema();
        FieldHierarchicalIterator iterator = new(schema.RootRecordTypes[0])
        {
            ExpandRepeatingFields = true
        };

        int counter = EnumerateFields(iterator);

        counter.Should().Be(10);
    }
}
