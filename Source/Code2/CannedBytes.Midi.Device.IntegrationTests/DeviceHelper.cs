using System;
using System.IO;

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
        DeviceProvider deviceProvider = DeviceProvider.Create(serviceProvider, schemaLocation);
        SchemaNodeMap binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

        DeviceToLogicalProcess process = new();

        FileStream stream = File.OpenRead(binStreamPath);
        DeviceDataContext dataCtx = process.Execute(binMap.RootNode, stream, writer);

        return dataCtx;
    }
}
