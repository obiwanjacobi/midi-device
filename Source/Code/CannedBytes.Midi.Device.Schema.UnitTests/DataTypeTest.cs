using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.DataType and is intended
    ///to contain all Jacobi.Midi.Device.Schema.DataType Unit Tests
    ///</summary>
    [TestClass()]
    public class DataTypeTest
    {
        private const string SchemaName = "urn:midi-test-schema";
        private const string TypeName = "TestType";

        [TestMethod()]
        public void DataType_ConstructorTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);

            SchemaObjectTest.AssertName(target, SchemaName, TypeName);
        }

        [TestMethod()]
        public void DataType_BaseTypeTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            DataType baseType = new DataType(SchemaName + ":BaseType");
            SchemaObjectTest.AssertName(baseType, SchemaName, "BaseType");

            target.BaseTypes.Add(baseType);

            Assert.IsTrue(target.HasBaseTypes);
            Assert.IsNotNull(target.BaseType);
            Assert.AreSame(baseType, target.BaseType);
            Assert.AreEqual(1, target.BaseTypes.Count);
        }

        [TestMethod()]
        public void DataType_IsTypeTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            DataType target = new DataType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            DataType baseType = new DataType(SchemaName + ":BaseType");
            SchemaObjectTest.AssertName(baseType, SchemaName, "BaseType");

            target.BaseTypes.Add(baseType);

            // own type should be found with/w-out recursive.
            Assert.IsTrue(target.IsType(fullName, false));
            Assert.IsTrue(target.IsType(fullName, true));
            // immediate base type should be found with/w-out recursive.
            Assert.IsTrue(target.IsType(SchemaName + ":BaseType", true));
            Assert.IsTrue(target.IsType(SchemaName + ":BaseType", false));
        }
    }
}