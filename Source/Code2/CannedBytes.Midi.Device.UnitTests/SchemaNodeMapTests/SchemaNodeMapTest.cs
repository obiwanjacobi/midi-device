using System;
using Xunit;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests.ConverterTests;
using FluentAssertions;
using System.IO;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.UnitTests.SchemaNodeMapTests
{
    
    //[DeploymentItem(Folder + SchemaNodeMapTestSchema)]
    public class SchemaNodeMapTest
    {
        public const string Folder = "SchemaNodeMapTests/";
        public const string SchemaNodeMapTestSchema = "SchemaNodeMapTestSchema.mds";
        
        private readonly ITestOutputHelper _output;

        public SchemaNodeMapTest(ITestOutputHelper output)
            => _output = output;

        //public TestContext TestContext { get; set; }

        public static SchemaNodeMap CreateSchemaNodeMap(DeviceSchema schema)
        {
            var converterMgr = ConverterManagerTest.CreateConverterManager();

            var factory = new SchemaNodeMapFactory(converterMgr);

            var map = factory.Create(schema.VirtualRootFields[0]);

            return map;
        }

        private void SaveSchemaNodeMap(SchemaNodeMap map, string name)
        {
            //var path = Path.Combine(TestContext.DeploymentDirectory, name);

            DgmlFactory.SaveGraph(map, name);
        }

        [Fact]
        public void Create_HierarchicalSchema_IsNotNull()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNodeMapTestSchema);
            var map = CreateSchemaNodeMap(schema);

            SaveSchemaNodeMap(map, "Create_HierarchicalSchema_IsNotNull");
            _output.WriteLine(map.ToString());

            map.Should().NotBeNull();
        }

        [Fact]
        public void Create_HierarchicalSchema_RootIsSet()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNodeMapTestSchema);
            var map = CreateSchemaNodeMap(schema);

            SaveSchemaNodeMap(map, "Create_HierarchicalSchema_RootIsSet");
            _output.WriteLine(map.ToString());

            map.RootNode.Should().NotBeNull();
            map.RootNode.IsRoot.Should().BeTrue();
            map.RootNode.IsRecord.Should().BeTrue();
        }

        [Fact]
        public void Create_HierarchicalSchema_LastNodeIsSet()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNodeMapTestSchema);
            var map = CreateSchemaNodeMap(schema);

            SaveSchemaNodeMap(map, "Create_HierarchicalSchema_LastNodeIsSet");
            _output.WriteLine(map.ToString());

            map.LastNode.Should().NotBeNull();
            map.LastNode.IsRoot.Should().BeFalse();
        }

        [Fact]
        public void Create_HierarchicalSchema_AddressMapIsSet()
        {
            var schema = DeviceSchemaHelper.LoadSchema(SchemaNodeMapTestSchema);
            var map = CreateSchemaNodeMap(schema);

            SaveSchemaNodeMap(map, "Create_HierarchicalSchema_AddressMapIsSet");
            _output.WriteLine(map.ToString());

            map.AddressMap.Should().NotBeNull();
            map.AddressMap.IsRecord.Should().BeTrue();
        }
    }
}
