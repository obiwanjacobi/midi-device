using System.IO;
using CannedBytes.Midi.Device.IntegrationTests;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CannedBytes.Midi.Device.UnitTests.AddressMapTests;

public class AddressMapTest
{
    public const string Folder = "AddressMapTests/";
    public const string AddressMapManagerTestSchema = "AddressMapTestSchema.mds";
    public const string AddressMapManagerTestStream21 = "AddressMapTestStream21.bin";
    
    public const string AddressIndex0 = "urn:AddressMapTestSchema:Address[0]";
    public const string SizeIndex0 = "urn:AddressMapTestSchema:Size[0]";
    public const string Field1bIndex1 = "urn:AddressMapTestSchema:Field1b[0|1|0]";
    public const string Field1cIndex1 = "urn:AddressMapTestSchema:Field1c[0|1|0]";

    private readonly ITestOutputHelper _output;

    public AddressMapTest(ITestOutputHelper output)
        => _output = output;

    private DeviceDataContext ReadLogical(string virtualRootName, IMidiLogicalWriter writer)
    {
        var serviceProvider = ServiceHelper.CreateServices();
        var ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, AddressMapManagerTestSchema),
            Path.Combine(Folder, AddressMapManagerTestStream21), virtualRootName, writer);

        ctx.Should().NotBeNull();
        _output.WriteLine(ctx.LogManager.ToString());

        return ctx;
    }

    [Fact]
    public void ReadLogical_Address21_Size2_Field1b_Field1c()
    {
        var writer = new DictionaryBasedLogicalStub();
        var ctx = ReadLogical("RootMessage", writer);

        writer.Should().HaveCount(4);
        writer[0].Key.Should().Be(AddressIndex0);
        writer[1].Key.Should().Be(SizeIndex0);
        writer[2].Key.Should().Be(Field1bIndex1);
        writer[3].Key.Should().Be(Field1cIndex1);
    }
}
