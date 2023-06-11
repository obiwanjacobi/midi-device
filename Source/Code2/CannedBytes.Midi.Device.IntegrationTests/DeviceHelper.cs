using System;
using System.IO;
using CannedBytes.Midi.Device.IntegrationTests.Stubs;

namespace CannedBytes.Midi.Device.IntegrationTests;

internal static class DeviceHelper
{
    public static DeviceDataContext ToLogical(
        IServiceProvider serviceProvider,
        string schemaLocation,
        string binStreamPath,
        string virtualRootName,
        IMidiLogicalWriter writer)
    {
        var deviceProvider = DeviceProvider.Create(serviceProvider, schemaLocation);
        var binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

        DeviceToLogicalProcess process = new();

        using var stream = File.OpenRead(binStreamPath);
        var dataCtx = process.Execute(binMap.RootNode, stream, writer);

        return dataCtx;
    }
}
