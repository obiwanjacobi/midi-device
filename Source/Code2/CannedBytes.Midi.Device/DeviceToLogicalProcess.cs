using System.IO;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Single class to manage going from a physical stream to logical values
/// </summary>
public class DeviceToLogicalProcess
{
    public virtual DeviceDataContext Execute(SchemaNode rootNode, Stream physicalStream, IMidiLogicalWriter logicalWriter)
    {
        DeviceDataContext context = CreateContext(physicalStream, rootNode);
        ProcessToLogical navigator = new(context, rootNode);

        navigator.ToLogical(logicalWriter);

        return context;
    }

    protected virtual DeviceDataContext CreateContext(Stream physicalStream, SchemaNode rootNode)
    {
        DeviceDataContext ctx = new(ConversionDirection.ToLogical)
        {
            RootNode = rootNode,
            StreamManager = new StreamManager(physicalStream)
        };

        return ctx;
    }
}
