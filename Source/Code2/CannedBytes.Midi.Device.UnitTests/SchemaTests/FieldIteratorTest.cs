using System;
using Xunit;
using CannedBytes.Midi.Device.Schema;
using FluentAssertions;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests
{
    
    //[DeploymentItem(Folder + HierarchicalSchema)]
    public class FieldIteratorTest
    {
        public const string Folder = "SchemaTests/";
        public const string HierarchicalSchema = "HierarchicalSchema.mds";

        [Fact]
        public void MoveNext_SchemaRecordType_VerifyAllFields()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldIterator(schema.AllRecordTypes.Find("subRecord"), 2);

            var counter = FieldHierarchicalIteratorTest.EnumerateFields(iterator);

            counter.Should().Be(4);
        }

        [Fact]
        public void MoveNext_SchemaRecordType_VerifyFieldInstanceIndex()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldIterator(schema.AllRecordTypes.Find("subRecord"), 2);

            int counter = 0;

            foreach (var fieldInfo in iterator)
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
}
