using CannedBytes.Midi.Device.Schema;
using Xunit;
using System;
using FluentAssertions;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device.UnitTests.SchemaTests
{
    
    //[DeploymentItem(Folder + HierarchicalSchema)]
    public class FieldHierarchicalIteratorTest
    {
        public const string Folder = "SchemaTests/";
        public const string HierarchicalSchema = "HierarchicalSchema.mds";

        public static int EnumerateFields(IEnumerable<FieldInfo> iterator)
        {
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

                counter++;
            }

            return counter;
        }

        [Fact]
        public void CheckLoadSchema_HierarchicalSchema_LoadsWithoutErrors()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);

            schema.Should().NotBeNull();
            schema.RootRecordTypes.Should().NotBeEmpty();
        }

        [Fact]
        public void MoveNext_HierarchicalSchema_EnumeratorLoopsOnce()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldHierarchicalIterator(schema.RootRecordTypes[0]);

            var enumerator = iterator.GetEnumerator();
            
            enumerator.Should().NotBeNull();
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().NotBeNull();
        }
        
        [Fact]
        public void MoveNext_HierarchicalSchema_ReturnsFields()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldHierarchicalIterator(schema.RootRecordTypes[0]);

            foreach (var field in iterator)
            {
                field.Should().NotBeNull();
            }
        }

        [Fact]
        public void MoveNext_HierarchicalSchema_VerifyAllFields()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldHierarchicalIterator(schema.RootRecordTypes[0]);

            int counter = EnumerateFields(iterator);

            counter.Should().Be(6);
        }

        

        [Fact]
        public void MoveNext_HierarchicalSchema_VerifyAllRepeatedFields()
        {
            var schema = DeviceSchemaHelper.LoadSchema(HierarchicalSchema);
            var iterator = new FieldHierarchicalIterator(schema.RootRecordTypes[0]);

            iterator.ExpandRepeatingFields = true;

            int counter = EnumerateFields(iterator);

            counter.Should().Be(10);
        }
    }
}
