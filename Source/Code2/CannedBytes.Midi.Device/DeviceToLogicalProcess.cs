using System.IO;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Single class to manage going from a physical stream to logical values
/// </summary>
public partial class DeviceToLogicalProcess
{
    public virtual DeviceDataContext Execute(SchemaNode rootNode, Stream physicalStream, IMidiLogicalWriter logicalWriter)
    {
        var context = CreateContext(physicalStream, rootNode);
        var navigator = new ProcessToLogical(context, rootNode);

        navigator.ToLogical(logicalWriter);

        return context;
    }

    protected virtual LogicalDeviceDataContext CreateContext(Stream physicalStream, SchemaNode rootNode)
    {
        var ctx = new LogicalDeviceDataContext(rootNode, new StreamManager(physicalStream));
        return ctx;
    }
}
