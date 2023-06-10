using System;

namespace CannedBytes.Midi.Device;

/// <summary>
/// This class iterates the nodes in the schema and calls each converter ToPhysical method.
/// </summary>
public class ProcessToPhysical
{
    // TODO: possibly make a base class for overlap with ProcessToLogical.

    private readonly DeviceDataContext _context;
    private readonly SchemaNode _rootNode;

    public ProcessToPhysical(DeviceDataContext context, SchemaNode rootNode)
    {
        _context = context;
        _rootNode = rootNode;
    }

    public SchemaNode CurrentNode
    {
        get { return _context.FieldInfo.CurrentNode; }
        set { _context.FieldInfo.CurrentNode = value; }
    }

    public void ToPhysical(IMidiLogicalReader reader)
    {
        //_context.LogicalReadAccessor = new LogicalReadAccessor(_context, reader);

        ToPhysical(_rootNode);
    }

    private void ToPhysical(SchemaNode thisNode)
    {
        throw new NotImplementedException();

        //if (thisNode.IsRecord)
        //{
        //    RecordToPhysical(thisNode);
        //}
        //else
        //{
        //    FieldToPhysical(thisNode);
        //}
    }
}
