using System.IO;
using System.Linq;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.UnitTests.SchemaNodeMapTests;
using FluentAssertions;
using Xunit;

namespace CannedBytes.Midi.Device.UnitTests.AddressMapTests;

public class AddressMapManagerTest
{
    public const string Folder = "AddressMapTests/";
    public const string AddressMapManagerTestSchema = "AddressMapTestSchema.mds";

    private static AddressMapManager CreateAddressMapManager(DeviceSchema schema)
    {
        var map = SchemaNodeMapTest.CreateSchemaNodeMap(schema);

        return new AddressMapManager(map.RootNode);
    }

    private static AddressMapManager CreateAddressMapManager()
    {
        var path = Path.Combine(Folder, AddressMapManagerTestSchema);
        var schema = DeviceSchemaHelper.LoadSchemaFile(path);
        var mgr = CreateAddressMapManager(schema);
        return mgr;
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize5_GivesFourNodesWithFixedEndNode()
    {
        // record starts at address 10H and repeats every 10H bytes
        // so a length of 05H bytes will be truncated to the last field
        // (Field1c) of the first InstanceIndex.
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(5);

        var result = mgr.CreateSchemaNodes(address, size).ToList();

        result.Should().HaveCount(4);

        result[0].Address.Should().Be(0x10);
        result[0].Field.Name.Name.Should().Be("Field1");
        result[0].InstanceIndex.Should().Be(0);
        result[0].IsRecord.Should().BeTrue();

        result[1].Address.Should().Be(0x10);
        result[1].Field.Name.Name.Should().Be("Field1a");
        result[1].InstanceIndex.Should().Be(0);

        result[2].Address.Should().Be(0x11);
        result[2].Field.Name.Name.Should().Be("Field1b");
        result[2].InstanceIndex.Should().Be(0);

        result[3].Address.Should().Be(0x12);
        result[3].Field.Name.Name.Should().Be("Field1c");
        result[3].InstanceIndex.Should().Be(0);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize11h_GivesSevenNodes()
    {
        // record starts at address 10H and repeats every 10H bytes
        // so a length of 11H bytes will end with the first field (Field1a)
        // of the second InstanceIndex
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(0x11);

        var result = mgr.CreateSchemaNodes(address, size).ToList();

        result.Should().HaveCount(6);

        result[0].Address.Should().Be(0x10);
        result[0].Field.Name.Name.Should().Be("Field1");
        result[0].InstanceIndex.Should().Be(0);
        result[0].IsRecord.Should().BeTrue();

        result[1].Address.Should().Be(0x10);
        result[1].Field.Name.Name.Should().Be("Field1a");
        result[1].InstanceIndex.Should().Be(0);

        result[2].Address.Should().Be(0x11);
        result[2].Field.Name.Name.Should().Be("Field1b");
        result[2].InstanceIndex.Should().Be(0);

        result[3].Address.Should().Be(0x12);
        result[3].Field.Name.Name.Should().Be("Field1c");
        result[3].InstanceIndex.Should().Be(0);

        // -- second instance at address 20H

        result[4].Address.Should().Be(0x20);
        result[4].Field.Name.Name.Should().Be("Field1");
        result[4].InstanceIndex.Should().Be(1);
        result[4].IsRecord.Should().BeTrue();

        result[5].Address.Should().Be(0x20);
        result[5].Field.Name.Name.Should().Be("Field1a");
        result[5].InstanceIndex.Should().Be(1);
    }

    [Fact]
    public void CreateSchemaNodes_Address10hAndSize1_GivesTwoNodes()
    {
        // record starts at address 10H and repeats every 10H bytes
        // so a length of 01H bytes will end with the first field (Field1a)
        // of the first InstanceIndex
        var mgr = CreateAddressMapManager();

        var address = SevenBitUInt32.FromSevenBitValue(0x10);
        var size = SevenBitUInt32.FromSevenBitValue(0x1);

        var result = mgr.CreateSchemaNodes(address, size).ToList();

        // two nodes: includes parent record and field itself
        result.Should().HaveCount(2);

        result[0].Address.Should().Be(0x10);
        result[0].Field.Name.Name.Should().Be("Field1");
        result[0].InstanceIndex.Should().Be(0);
        result[0].IsRecord.Should().BeTrue();

        result[1].Address.Should().Be(0x10);
        result[1].Field.Name.Name.Should().Be("Field1a");
        result[1].InstanceIndex.Should().Be(0);
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
