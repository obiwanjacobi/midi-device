using System.IO;
using CannedBytes.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.IntegrationTests;

internal static class DeviceHelper
{
    public static DeviceDataContext ToLogical(
        CompositionContext compositionCtx,
        string schemaLocation,
        string binStreamPath,
        string virtualRootName,
        IMidiLogicalWriter writer)
    {
        DeviceProvider deviceProvider = DeviceProvider.Create(compositionCtx, schemaLocation);
        SchemaNodeMap binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

        DeviceToLogicalProcess process = new();

        DeviceDataContext dataCtx = null;

        using (FileStream stream = File.OpenRead(binStreamPath))
        {
            dataCtx = process.Execute(binMap.RootNode, stream, writer);
        }

        return dataCtx;
    }
}
