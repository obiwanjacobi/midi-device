using System;
using CannedBytes.Midi.Device.Schema;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests
{
    internal static class AssertDeviceSchemaHelper
    {
        internal static void AssertDeviceSchema(DeviceSchema schema)
        {
            Assert.NotNull(schema);
            //Assert.True(schema.AllDataTypes.Count > 0, "DeviceSchema has no DataTypes.");
            Assert.True(schema.AllRecordTypes.Count > 0, "DeviceSchema has no RecordTypes.");
            Assert.True(schema.RootRecordTypes.Count > 0, "DeviceSchema has no root RecordTypes.");
            Assert.False(String.IsNullOrEmpty(schema.SchemaName), "DeviceSchema has no name.");

            AssertDataTypeCollection(schema, schema.AllDataTypes);
            AssertRecordTypeCollection(schema, schema.AllRecordTypes);
            AssertRecordTypeCollection(schema, schema.RootRecordTypes);
        }

        internal static void AssertRecordTypeCollection(DeviceSchema schema, RecordTypeCollection recordTypes)
        {
            AssertSchemaObjectCollection(schema, recordTypes);

            foreach (RecordType recordType in recordTypes)
            {
                AssertRecordType(schema, recordType);
            }
        }

        internal static void AssertRecordType(DeviceSchema schema, RecordType recordType)
        {
            AssertSchemaObject(schema, recordType);
            AssertSchemaObjectCollection(schema, recordType.Attributes);

            //if (recordType.BaseType != null)
            //{
            //    AssertRecordType(schema, recordType.BaseType);
            //}

            foreach (Field field in recordType.Fields)
            {
                AssertField(schema, field);
            }
        }

        internal static void AssertField(DeviceSchema schema, Field field)
        {
            AssertSchemaObject(schema, field);
            AssertSchemaObjectCollection(schema, field.Attributes);

            if (field.DataType != null)
            {
                AssertImportedDataType(field.DataType);
            }
            if (field.RecordType != null)
            {
                AssertRecordType(schema, field.RecordType);
            }

            Assert.NotNull(field.DeclaringRecord);

            // causes recursion
            //AssertRecordType(schema, field.DeclaringRecord);

            Assert.True(field.Repeats >= 1, "Field has an invalid Repeats value.");
        }

        internal static void AssertDataTypeCollection(DeviceSchema schema, DataTypeCollection dataTypes)
        {
            AssertSchemaObjectCollection(schema, dataTypes);

            foreach (DataType datatype in dataTypes)
            {
                AssertDataType(schema, datatype);
            }
        }

        internal static void AssertDataType(DeviceSchema schema, DataType dataType)
        {
            AssertSchemaObject(schema, dataType);
            AssertSchemaObjectCollection(schema, dataType.Attributes);

            Assert.Equal(dataType.HasBaseTypes, dataType.BaseType != null);

            if (dataType.HasBaseTypes)
            {
                Assert.True(dataType.BaseTypes.Count > 0, "DataType has a BaseType but BaseTypes collection is empty.");

                foreach (DataType baseType in dataType.BaseTypes)
                {
                    AssertImportedDataType(baseType);
                }
            }
        }

        internal static void AssertImportedDataType(DataType dataType)
        {
            AssertSchemaObject(null, dataType);
            AssertSchemaObjectCollection(null, dataType.Attributes);

            Assert.Equal(dataType.HasBaseTypes, dataType.BaseType != null);

            if (dataType.HasBaseTypes)
            {
                Assert.True(dataType.BaseTypes.Count > 0, "Imported DataType has a BaseType but BaseTypes collection is empty.");

                foreach (DataType baseType in dataType.BaseTypes)
                {
                    AssertImportedDataType(baseType);
                }
            }
        }

        internal static void AssertSchemaObjectName(DeviceSchema schema, SchemaObjectName name)
        {
            Assert.False(String.IsNullOrEmpty(name.FullName), "SchemaObjectName has no FullName.");
            Assert.False(String.IsNullOrEmpty(name.Name), "SchemaObjectName has no Name.");
            Assert.False(String.IsNullOrEmpty(name.SchemaName), "SchemaObjectName has no SchemaName.");

            if (schema != null)
            {
                Assert.Same(schema.SchemaName, name.SchemaName);
            }
        }

        internal static void AssertSchemaObject(DeviceSchema schema, SchemaObject schemaObj)
        {
            if (schema != null)
            {
                Assert.NotNull(schemaObj.Schema);
                Assert.Equal(schema, schemaObj.Schema);
                Assert.Same(schema.SchemaName, schemaObj.Schema.SchemaName);
            }

            AssertSchemaObjectName(schema, schemaObj.Name);
        }

        internal static void AssertSchemaObjectCollection<T>(DeviceSchema schema, SchemaCollection<T> collection) where T : SchemaObject
        {
            Assert.NotNull(collection.Schema);

            if (schema != null)
            {
                Assert.Equal(schema, collection.Schema);
                Assert.Same(schema.SchemaName, collection.Schema.SchemaName);
            }
        }
    }
}