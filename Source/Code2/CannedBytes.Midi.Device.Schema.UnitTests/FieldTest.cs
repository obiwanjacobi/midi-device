using Xunit;

namespace CannedBytes.Midi.Device.Schema.UnitTests
{
    /// <summary>
    /// This is a test class for Jacobi.Midi.Device.Schema.Field and is intended
    /// to contain all Jacobi.Midi.Device.Schema.Field Unit Tests
    /// </summary>
    
    public class FieldTest
    {
        [Fact]
        public void Constructor_WithName_ParsedCorrectly()
        {
            var target = new Field(Constants.SchemaFieldName);

            SchemaObjectTest.AssertName(target, Constants.SchemaName, Constants.FieldName);
        }
    }
}