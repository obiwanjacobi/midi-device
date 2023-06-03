using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    /// Summary description for SchemaObjectTest
    /// </summary>
    [TestClass]
    public class SchemaObjectTest
    {
        public static void AssertName(SchemaObject schemaObject, string schemaName, string objName)
        {
            Assert.IsNotNull(schemaObject);
            Assert.IsNotNull(schemaObject.Name);
            Assert.AreEqual(schemaName, schemaObject.Name.SchemaName);
            Assert.AreEqual(objName, schemaObject.Name.Name);
            Assert.AreEqual(schemaName + ":" + objName, schemaObject.Name.FullName);
        }
    }
}