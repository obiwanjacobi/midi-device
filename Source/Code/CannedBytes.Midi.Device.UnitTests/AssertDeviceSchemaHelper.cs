using System;
using CannedBytes.Midi.Device.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.UnitTests
{
    internal static class AssertDeviceSchemaHelper
    {
        internal static void AssertDeviceSchema(DeviceSchema schema)
        {
            Assert.IsNotNull(schema, "DeviceSchema reference is null.");
            //Assert.IsTrue(schema.AllDataTypes.Count > 0, "DeviceSchema has no DataTypes.");
            Assert.IsTrue(schema.AllRecordTypes.Count > 0, "DeviceSchema has no RecordTypes.");
            Assert.IsTrue(schema.RootRecordTypes.Count > 0, "DeviceSchema has no root RecordTypes.");
            Assert.IsFalse(String.IsNullOrEmpty(schema.SchemaName), "DeviceSchema has no name.");

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

            Assert.IsNotNull(field.DeclaringRecord, "Field does not have a declaring RecordType.");

            // causes recursion
            //AssertRecordType(schema, field.DeclaringRecord);

            Assert.IsTrue(field.Repeats >= 1, "Field has an invalid Repeats value.");
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

            Assert.AreEqual(dataType.HasBaseTypes, dataType.BaseType != null, "DataType.HasBaseTypes does not report correct state.");

            if (dataType.HasBaseTypes)
            {
                Assert.IsTrue(dataType.BaseTypes.Count > 0, "DataType has a BaseType but BaseTypes collection is empty.");

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

            Assert.AreEqual(dataType.HasBaseTypes, dataType.BaseType != null, "Imported DataType.HasBaseTypes does not report correct state.");

            if (dataType.HasBaseTypes)
            {
                Assert.IsTrue(dataType.BaseTypes.Count > 0, "Imported DataType has a BaseType but BaseTypes collection is empty.");

                foreach (DataType baseType in dataType.BaseTypes)
                {
                    AssertImportedDataType(baseType);
                }
            }
        }

        internal static void AssertSchemaObjectName(DeviceSchema schema, SchemaObjectName name)
        {
            Assert.IsFalse(String.IsNullOrEmpty(name.FullName), "SchemaObjectName has no FullName.");
            Assert.IsFalse(String.IsNullOrEmpty(name.Name), "SchemaObjectName has no Name.");
            Assert.IsFalse(String.IsNullOrEmpty(name.SchemaName), "SchemaObjectName has no SchemaName.");

            if (schema != null)
            {
                Assert.AreSame(schema.SchemaName, name.SchemaName, "SchemaObjectName does not belong to the correct DeviceSchema.");
            }
        }

        internal static void AssertSchemaObject(DeviceSchema schema, SchemaObject schemaObj)
        {
            if (schema != null)
            {
                Assert.IsNotNull(schemaObj.Schema, "SchemaObject does not belong to a DeviceSchema.");
                Assert.AreEqual(schema, schemaObj.Schema, "SchemaObject does not belong to correct DeviceSchema (ref).");
                Assert.AreSame(schema.SchemaName, schemaObj.Schema.SchemaName, "SchemaObject does not belong to correct DeviceSchema (name).");
            }

            AssertSchemaObjectName(schema, schemaObj.Name);
        }

        internal static void AssertSchemaObjectCollection<T>(DeviceSchema schema, SchemaCollection<T> collection) where T : SchemaObject
        {
            Assert.IsNotNull(collection.Schema, "SchemaCollection does not belong to a DeviceSChema.");

            if (schema != null)
            {
                Assert.AreEqual(schema, collection.Schema, "SchemaCollection does not belong to correct DeviceSchema (ref).");
                Assert.AreSame(schema.SchemaName, collection.Schema.SchemaName, "SchemaCollection does not belong to correct DeviceSchema (name).");
            }
        }
    }
}