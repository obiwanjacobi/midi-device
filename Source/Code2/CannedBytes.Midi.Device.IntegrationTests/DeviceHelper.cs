using CannedBytes.ComponentModel.Composition;
using System.IO;

namespace CannedBytes.Midi.Device.IntegrationTests
{
    internal static class DeviceHelper
    {
        public static DeviceDataContext ToLogical(
            CompositionContext compositionCtx, 
            string schemaLocation,
            string binStreamPath,
            string virtualRootName,
            IMidiLogicalWriter writer)
        {
            var deviceProvider = DeviceProvider.Create(compositionCtx, schemaLocation);
            var binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

            var process = new DeviceToLogicalProcess();

            DeviceDataContext dataCtx = null;

            using (var stream = File.OpenRead(binStreamPath))
            {
                dataCtx = process.Execute(binMap.RootNode, stream, writer);
            }

            return dataCtx;
        }
    }
}
