using System.IO;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Single class to manage going from logical values to a physical stream.
/// </summary>
public partial class DeviceToPhysicalProcess
{
    public virtual DeviceDataContext Execute(SchemaNode rootNode, Stream physicalStream, IMidiLogicalReader logicalReader)
    {
        var context = CreateContext(physicalStream, rootNode);
        var navigator = new ProcessToPhysical(context, rootNode);

        navigator.ToPhysical(logicalReader);

        return context;
    }

    protected virtual PhysicalDeviceDataContext CreateContext(Stream physicalStream, SchemaNode rootNode)
    {
        var ctx = new PhysicalDeviceDataContext(rootNode, new StreamManager(physicalStream));
        return ctx;
    }
}
