using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Used to access information on the binary nodes
/// </summary>
public interface ILogicalFieldInfo
{
    /// <summary>
    /// Gets the index of this instance.
    /// </summary>
    Field Field { get; }

    /// <summary>
    /// Parent record for the field.
    /// </summary>
    ILogicalFieldInfo? Parent { get; }

    /// <summary>
    /// Gets a unique key value for the current node (Field position).
    /// </summary>
    InstancePathKey Key { get; }
}
