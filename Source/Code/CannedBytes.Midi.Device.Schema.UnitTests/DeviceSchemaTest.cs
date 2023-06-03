using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.DeviceSchema and is intended
    ///to contain all Jacobi.Midi.Device.Schema.DeviceSchema Unit Tests
    ///</summary>
    [TestClass()]
    public class DeviceSchemaTest
    {
        private const string SchemaName = "urn:midi-test-schema";

        [TestMethod]
        public void DeviceSchema_ConstructorTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            Assert.AreEqual(SchemaName, target.SchemaName);
        }

        [TestMethod]
        public void DeviceSchema_DataTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            Assert.AreEqual(SchemaName, target.SchemaName);
            Assert.IsNotNull(target.AllDataTypes);

            string typeName = SchemaName + ":DataType";
            DataType type = new DataType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            Assert.IsNull(type.Schema);

            target.AllDataTypes.Add(type);

            Assert.IsNotNull(type.Schema);
            Assert.AreSame(target, type.Schema);
            Assert.AreEqual(1, target.AllDataTypes.Count);
            Assert.IsNotNull(target.AllDataTypes[0]);
            Assert.AreSame(type, target.AllDataTypes[0]);

            target.AllDataTypes.Clear();

            Assert.AreEqual(0, target.AllDataTypes.Count);
        }

        [TestMethod]
        public void DeviceSchema_RecordTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            Assert.AreEqual(SchemaName, target.SchemaName);
            Assert.IsNotNull(target.AllDataTypes);

            string typeName = SchemaName + ":DataType";
            RecordType type = new RecordType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            Assert.IsNull(type.Schema);

            target.AllRecordTypes.Add(type);

            Assert.IsNotNull(type.Schema);
            Assert.AreEqual(1, target.AllRecordTypes.Count);
            Assert.IsNotNull(target.AllRecordTypes[0]);
            Assert.AreSame(type, target.AllRecordTypes[0]);

            target.AllRecordTypes.Clear();

            Assert.AreEqual(0, target.AllRecordTypes.Count);
        }

        [TestMethod]
        public void DeviceSchema_RootTypesTest()
        {
            DeviceSchema target = new DeviceSchema(SchemaName);

            Assert.AreEqual(SchemaName, target.SchemaName);
            Assert.IsNotNull(target.AllDataTypes);

            string typeName = SchemaName + ":DataType";
            RecordType type = new RecordType(typeName);

            SchemaObjectTest.AssertName(type, SchemaName, "DataType");
            Assert.IsNull(type.Schema);

            target.RootRecordTypes.Add(type);

            Assert.IsNotNull(type.Schema);
            Assert.AreEqual(1, target.RootRecordTypes.Count);
            Assert.IsNotNull(target.RootRecordTypes[0]);
            Assert.AreSame(type, target.RootRecordTypes[0]);

            target.RootRecordTypes.Clear();

            Assert.AreEqual(0, target.RootRecordTypes.Count);
        }
    }
}