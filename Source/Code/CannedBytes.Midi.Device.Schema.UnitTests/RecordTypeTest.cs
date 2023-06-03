using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.RecordType and is intended
    ///to contain all Jacobi.Midi.Device.Schema.RecordType Unit Tests
    ///</summary>
    [TestClass()]
    public class RecordTypeTest
    {
        private const string SchemaName = "urn:midi-test-schema";
        private const string TypeName = "TestType";

        [TestMethod()]
        public void RecordType_ConstructorTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            RecordType target = new RecordType(fullName);

            SchemaObjectTest.AssertName(target, SchemaName, TypeName);
        }

        [TestMethod()]
        public void RecordType_FieldTest()
        {
            string fullName = SchemaName + ":" + TypeName;

            RecordType target = new RecordType(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, TypeName);

            Assert.IsNotNull(target.Fields);

            string fieldName = SchemaName + ":TestField";

            Field field = new Field(fieldName);
            SchemaObjectTest.AssertName(field, SchemaName, "TestField");

            target.Fields.Add(field);

            Assert.AreEqual(1, target.Fields.Count);
            Assert.IsNotNull(target.Fields[0]);
            Assert.AreSame(field, target.Fields[0]);
        }
    }
}