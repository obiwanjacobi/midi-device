using System;
using System.IO;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests.ConverterTests;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.UnitTests.SchemaNodeMapTests;

public class SchemaNodeMapTest
{
    public const string Folder = "SchemaNodeMapTests/";
    public const string SchemaNodeMapTestSchema = "SchemaNodeMapTestSchema.mds";

    private readonly ITestOutputHelper _output;

    public SchemaNodeMapTest(ITestOutputHelper output)
        => _output = output;

    private static void SaveSchemaNodeMap(SchemaNodeMap map, string name)
    {
        string path = Path.Combine(Folder, name);
        DgmlFactory.SaveGraph(map, path);
    }

    private static SchemaNodeMap CreateSchemaNodeMap(string name)
    {
        string path = Path.Combine(Folder, name);
        DeviceSchema schema = DeviceSchemaHelper.LoadSchema(path);
        return CreateSchemaNodeMap(schema);
    }

    public static SchemaNodeMap CreateSchemaNodeMap(DeviceSchema schema)
    {
        Converters.ConverterManager converterMgr = ConverterManagerTest.CreateConverterManager();
        SchemaNodeMapFactory factory = new(converterMgr);
        SchemaNodeMap map = factory.Create(schema.VirtualRootFields[0]);
        return map;
    }

    [Fact]
    public void Create_HierarchicalSchema_IsNotNull()
    {
        SchemaNodeMap map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map, "Create_HierarchicalSchema_IsNotNull");
        _output.WriteLine(map.ToString());

        map.Should().NotBeNull();
    }

    [Fact]
    public void Create_HierarchicalSchema_RootIsSet()
    {
        SchemaNodeMap map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map, "Create_HierarchicalSchema_RootIsSet");
        _output.WriteLine(map.ToString());

        map.RootNode.Should().NotBeNull();
        map.RootNode.IsRoot.Should().BeTrue();
        map.RootNode.IsRecord.Should().BeTrue();
    }

    [Fact]
    public void Create_HierarchicalSchema_LastNodeIsSet()
    {
        SchemaNodeMap map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map, "Create_HierarchicalSchema_LastNodeIsSet");
        _output.WriteLine(map.ToString());

        map.LastNode.Should().NotBeNull();
        map.LastNode.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void Create_HierarchicalSchema_AddressMapIsSet()
    {
        SchemaNodeMap map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map, "Create_HierarchicalSchema_AddressMapIsSet");
        _output.WriteLine(map.ToString());

        map.AddressMap.Should().NotBeNull();
        map.AddressMap.IsRecord.Should().BeTrue();
    }
}
