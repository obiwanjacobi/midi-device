using System.IO;
using System.Runtime.CompilerServices;
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
    public const string SchemaNodeMapKeyBug = "SchemaNodeMapKeyBug.mds";

    private readonly ITestOutputHelper _output;

    public SchemaNodeMapTest(ITestOutputHelper output)
        => _output = output;

    private static void SaveSchemaNodeMap(SchemaNodeMap map, [CallerMemberName] string? callerName = null)
    {
        string path = Path.Combine(Folder, callerName);
        DgmlFactory.SaveGraph(map, path);
    }

    private static DeviceSchema LoadSchema(string name)
    {
        string path = Path.Combine(Folder, name);
        var schema = DeviceSchemaHelper.LoadSchemaFile(path);
        return schema;
    }

    private static SchemaNodeMap CreateSchemaNodeMap(string name)
    {
        var schema = LoadSchema(name);
        return CreateSchemaNodeMap(schema);
    }

    public static SchemaNodeMap CreateSchemaNodeMap(DeviceSchema schema)
    {
        var converterMgr = ConverterManagerTest.CreateConverterManager();
        SchemaNodeMapFactory factory = new(converterMgr);
        var map = factory.Create(schema.VirtualRootFields[0]);
        return map;
    }

    [Fact]
    public void Create_HierarchicalSchema_IsNotNull()
    {
        var map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map);
        _output.WriteLine(map.ToString());

        map.Should().NotBeNull();
    }

    [Fact]
    public void Create_HierarchicalSchema_RootIsSet()
    {
        var map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map);
        _output.WriteLine(map.ToString());

        map.RootNode.Should().NotBeNull();
        map.RootNode.IsRoot.Should().BeTrue();
        map.RootNode.IsRecord.Should().BeTrue();
    }

    [Fact]
    public void Create_HierarchicalSchema_LastNodeIsSet()
    {
        var map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map);
        _output.WriteLine(map.ToString());

        map.LastNode.Should().NotBeNull();
        map.LastNode.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void Create_HierarchicalSchema_AddressMapIsSet()
    {
        var map = CreateSchemaNodeMap(SchemaNodeMapTestSchema);

        SaveSchemaNodeMap(map);
        _output.WriteLine(map.ToString());

        map.AddressMap.Should().NotBeNull();
        map.AddressMap!.IsRecord.Should().BeTrue();
    }

    [Fact]
    public void InstanceKeyIndexBug_FirstOfSubSubRecordsWrong()
    {
        var map = CreateSchemaNodeMap(SchemaNodeMapKeyBug);

        SaveSchemaNodeMap(map);
        _output.WriteLine(map.ToString());

        map.AddressMap.Should().NotBeNull();
        map.AddressMap!.IsRecord.Should().BeTrue();

        var patch0 = map.AddressMap;
        var ctrlParam0_0 = patch0.Children[1];
        ctrlParam0_0.Key.ToString().Should().Be("0|0");
        ctrlParam0_0.NextClone!.Key.ToString().Should().Be("1|0");
        var patch1 = patch0.NextClone!;
        var ctrlParam1_0 = patch1.Children[1];
        ctrlParam1_0.Key.ToString().Should().Be("1|0");
        ctrlParam1_0.NextClone!.Key.ToString().Should().Be("1|1");
    }
}
