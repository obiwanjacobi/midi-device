using System.Collections.Generic;

namespace CannedBytes.Midi.Device;

// can be implemented by StreamConverters to override normal field processing
public interface ILogicalFieldNodeFactory
{
    IEnumerable<LogicalFieldNode> CreateLogicalFieldNodes(DeviceDataContext context);
}
