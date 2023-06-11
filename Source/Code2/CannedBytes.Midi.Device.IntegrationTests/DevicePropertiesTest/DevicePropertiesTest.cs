using CannedBytes.Midi.Device.IntegrationTests.Stubs;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using System.IO;

namespace CannedBytes.Midi.Device.IntegrationTests.DevicePropertiesTest;

public class DevicePropertiesTest
{
    public const string Folder = "DevicePropertiesTest/";
    public const string SchemaFileName = "DevicePropertiesTestSchema.mds";
    public const string StreamFileName = "DevicePropertiesTestStream.bin";
    
    private readonly ITestOutputHelper _output;

    public DevicePropertiesTest(ITestOutputHelper output)
        => _output = output;

    [Fact]
    public void ToLogical_SchemaWithProperties_LogicalPropertyValuesInContext()
    {
        DictionaryBasedLogicalStub writer = new();

        //ComponentModel.Composition.CompositionContext compositionCtx = CompositionHelper.CreateCompositionContext();
        //DeviceDataContext ctx = DeviceHelper.ToLogical(
        //    compositionCtx, SchemaFileName, StreamFileName, "RQ1", writer);

        var serviceProvider = ServiceHelper.CreateServices();
        DeviceDataContext ctx = DeviceHelper.ToLogical(serviceProvider,
            Path.Combine(Folder, SchemaFileName),
            Path.Combine(Folder, StreamFileName), "RQ1", writer);

        ctx.DeviceProperties.Count.Should().Be(4);
        ctx.DeviceProperties.Find("ManufacturerId").GetValue<int>().Should().Be(65);
        ctx.DeviceProperties.Find("SysExChannel").GetValue<int>().Should().Be(16);
        ctx.DeviceProperties.Find("ModelId").GetValue<int>().Should().Be(43);
        ctx.DeviceProperties.Find("CommandId").GetValue<int>().Should().Be(17);

        _output.WriteLine(ctx.RecordManager.ToString());
    }
}