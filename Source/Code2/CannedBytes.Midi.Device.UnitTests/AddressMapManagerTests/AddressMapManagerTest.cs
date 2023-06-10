using System.IO;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests.SchemaNodeMapTests;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.AddressMapManagerTests;

public class AddressMapManagerTest
{
    public const string Folder = "AddressMapManagerTests/";
    public const string AddressMapManagerTestSchema = "AddressMapManagerTestSchema.mds";

    public static AddressMapManager CreateAddressMapManager(DeviceSchema schema)
    {
        SchemaNodeMap map = SchemaNodeMapTest.CreateSchemaNodeMap(schema);

        return new AddressMapManager(map.RootNode);
    }

    private static AddressMapManager CreateAddressMapManager()
    {
        string path = Path.Combine(Folder, AddressMapManagerTestSchema);
        DeviceSchema schema = DeviceSchemaHelper.LoadSchema(path);
        AddressMapManager mgr = CreateAddressMapManager(schema);
        return mgr;
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize5_GivesFourNodesWithFixedEndNode()
    {
        AddressMapManager mgr = CreateAddressMapManager();

        SevenBitUInt32 address = SevenBitUInt32.FromSevenBitValue(0x10);
        SevenBitUInt32 size = SevenBitUInt32.FromSevenBitValue(5);

        System.Collections.Generic.IEnumerable<SchemaNode> result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize11h_GivesSevenNodes()
    {
        AddressMapManager mgr = CreateAddressMapManager();

        SevenBitUInt32 address = SevenBitUInt32.FromSevenBitValue(0x10);
        SevenBitUInt32 size = SevenBitUInt32.FromSevenBitValue(0x11);

        System.Collections.Generic.IEnumerable<SchemaNode> result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(6);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize1_GivesTwoNodes()
    {
        AddressMapManager mgr = CreateAddressMapManager();

        SevenBitUInt32 address = SevenBitUInt32.FromSevenBitValue(0x10);
        SevenBitUInt32 size = SevenBitUInt32.FromSevenBitValue(0x1);

        System.Collections.Generic.IEnumerable<SchemaNode> result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        // two nodes: includes parent record and field itself
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateSchemaNodes_Address0AndSize0_GivesAllNodes()
    {
        AddressMapManager mgr = CreateAddressMapManager();

        SevenBitUInt32 address = SevenBitUInt32.FromSevenBitValue(0x0);
        SevenBitUInt32 size = SevenBitUInt32.FromSevenBitValue(0x0);

        System.Collections.Generic.IEnumerable<SchemaNode> result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(15);
    }
}
