using System.IO;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests;

public class FieldIteratorTest
{
    public const string Folder = "SchemaTests/";
    public const string HierarchicalSchema = "HierarchicalSchema.mds";

    private static DeviceSchema LoadTestSchema()
    {
        string path = Path.Combine(Folder, HierarchicalSchema);
        return DeviceSchemaHelper.LoadSchema(path);
    }

    [Fact]
    public void MoveNext_SchemaRecordType_VerifyAllFields()
    {
        var schema = LoadTestSchema();
        FieldIterator iterator = new(schema.AllRecordTypes.Find("subRecord"), 2);

        int counter = FieldHierarchicalIteratorTest.EnumerateFields(iterator);

        counter.Should().Be(4);
    }

    [Fact]
    public void MoveNext_SchemaRecordType_VerifyFieldInstanceIndex()
    {
        var schema = LoadTestSchema();
        FieldIterator iterator = new(schema.AllRecordTypes.Find("subRecord"), 2);

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

            fieldInfo.InstanceIndex.Should().Be(counter / 2);

            counter++;
        }
    }
}
