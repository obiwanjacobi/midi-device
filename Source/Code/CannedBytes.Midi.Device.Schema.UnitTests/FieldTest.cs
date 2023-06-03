using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    ///This is a test class for Jacobi.Midi.Device.Schema.Field and is intended
    ///to contain all Jacobi.Midi.Device.Schema.Field Unit Tests
    ///</summary>
    [TestClass()]
    public class FieldTest
    {
        private const string SchemaName = "urn:midi-test-schema";
        private const string FieldName = "TestField";

        [TestMethod()]
        public void Field_ConstructorTest()
        {
            string fullName = SchemaName + ":" + FieldName;

            Field target = new Field(fullName);
            SchemaObjectTest.AssertName(target, SchemaName, FieldName);
        }
    }
}