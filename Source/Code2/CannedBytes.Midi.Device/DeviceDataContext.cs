using System;

namespace CannedBytes.Midi.Device;

public abstract partial class DeviceDataContext
{
    protected DeviceDataContext(ConversionDirection dir, SchemaNode rootNode, StreamManager streamManager)
    {
        RootNode = rootNode;
        StreamManager = streamManager;
        FieldInfo = new FieldContext(this);
        DeviceProperties = new DevicePropertyCollection();
        StateMap = new ConverterState(this);

        ConversionDirection = dir;
        LogManager = new DataLogManager(this);
    }

    /// <summary>
    /// Indicates the direction of conversion.
    /// </summary>
    public ConversionDirection ConversionDirection { get; }

    /// <summary>
    /// Contains the runtime device properties retrieved from the message.
    /// </summary>
    public DevicePropertyCollection DeviceProperties { get; }

    // Streams
    public StreamManager StreamManager { get; init; }

    // log records
    public DataLogManager LogManager { get; }

    // schema
    public SchemaNode RootNode { get;  }

    /// <summary>
    /// Contains field-specific context information.
    /// </summary>
    public FieldContext FieldInfo { get; }

    /// <summary>
    /// A place for converters to store state during a conversion run.
    /// </summary>
    public ConverterState StateMap { get; }

    /// <summary>
    /// Creates the context information that is passed to the application
    /// </summary>
    /// <returns>Never returns null.</returns>
    internal LogicalContext CreateLogicalContext(int bitLength)
    {
        if (FieldInfo.CurrentNode is null)
        {
            throw new InvalidOperationException(
                "No current node is set");
        }

        var ctx = new LogicalContext(FieldInfo.CurrentNode, bitLength);

        return ctx;
    }
}
