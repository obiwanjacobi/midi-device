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
        var map = SchemaNodeMapTest.CreateSchemaNodeMap(schema);

        return new AddressMapManager(map.RootNode);
    }

    private static AddressMapManager CreateAddressMapManager()
    {
        var path = Path.Combine(Folder, AddressMapManagerTestSchema);
        var schema = DeviceSchemaHelper.LoadSchema(path);
        var mgr = CreateAddressMapManager(schema);
        return mgr;
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize5_GivesFourNodesWithFixedEndNode()
    {
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(5);

        var result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize11h_GivesSevenNodes()
    {
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(0x11);

        var result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(6);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize1_GivesTwoNodes()
    {
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(0x1);

        var result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        // two nodes: includes parent record and field itself
        result.Should().HaveCount(2);
    }

    [Fact]
    public void CreateSchemaNodes_Address0AndSize0_GivesAllNodes()
    {
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x0);
        var size = SevenBitUInt32.FromSevenBitValue(0x0);

        var result = mgr.CreateSchemaNodes(address, size);

        result.Should().NotBeNull();
        result.Should().HaveCount(15);
    }
}
