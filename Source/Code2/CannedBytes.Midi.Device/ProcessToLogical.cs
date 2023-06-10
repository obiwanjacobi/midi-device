using System;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device;

/// <summary>
/// This class iterates the nodes in the schema and calls each converter ToLogical method.
/// </summary>
public class ProcessToLogical
{
    private readonly DeviceDataContext _context;
    private readonly SchemaNode _rootNode;

    public ProcessToLogical(DeviceDataContext context, SchemaNode rootNode)
    {
        _context = context;
        _rootNode = rootNode;
    }

    public SchemaNode CurrentNode
    {
        get { return _context.FieldInfo.CurrentNode; }
        set { _context.FieldInfo.CurrentNode = value; }
    }

    public void ToLogical(IMidiLogicalWriter writer)
    {
        _context.LogicalWriteAccessor = new LogicalWriteAccessor(_context, writer);

        ToLogical(_rootNode);
    }

    private void ToLogical(SchemaNode thisNode)
    {
        if (thisNode.IsRecord)
        {
            RecordToLogical(thisNode);
        }
        else
        {
            FieldToLogical(thisNode);
        }
    }

    private void RecordToLogical(SchemaNode thisNode)
    {
        using IDisposable scope = _context.RecordManager.BeginNewEntry();
        try
        {
            OnBeforeRecordToLogical(thisNode);
            OnRecordToLogical(thisNode);
            OnAfterRecordToLogical(thisNode);
        }
        catch (Exception e)
        {
            _context.RecordManager.SetError(e);

            throw;
        }
    }

    private void FieldToLogical(SchemaNode thisNode)
    {
        using IDisposable scope = _context.RecordManager.BeginNewEntry();
        try
        {
            OnBeforeFieldToLogical(thisNode);
            OnFieldToLogical(thisNode);
            OnAfterFieldToLogical(thisNode);
        }
        catch (Exception e)
        {
            _context.RecordManager.SetError(e);

            throw;
        }
    }

    protected virtual void OnBeforeRecordToLogical(SchemaNode thisNode)
    {
        ClearCarry();

        INavigationEvents navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnBeforeRecord(_context);
    }

    protected virtual void OnRecordToLogical(SchemaNode thisNode)
    {
        CurrentNode = thisNode;

        IEnumerable<SchemaNode> iterator = thisNode.FieldConverterPair.StreamConverter.GetChildNodeIterator(_context);

        foreach (SchemaNode node in iterator)
        {
            ToLogical(node);
        }
    }

    protected virtual void OnAfterRecordToLogical(SchemaNode thisNode)
    {
        INavigationEvents navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnAfterRecord(_context);

        // auto pop stream from the stack
        _context.StreamManager.RemoveCurrentStream(thisNode.FieldConverterPair.StreamConverter);

        ClearCarry();
    }

    protected virtual void OnBeforeFieldToLogical(SchemaNode thisNode)
    {
        INavigationEvents navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnBeforeField(_context);
    }

    protected virtual void OnFieldToLogical(SchemaNode thisNode)
    {
        CurrentNode = thisNode;

        DeviceStreamReader reader = _context.CreateReader();

        thisNode.FieldConverterPair.DataConverter.ToLogical(_context, reader, _context.LogicalWriteAccessor);
    }

    protected virtual void OnAfterFieldToLogical(SchemaNode thisNode)
    {
        INavigationEvents navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnAfterField(_context);
    }

    private void ClearCarry()
    {
        _context.Carry.Clear();

        if (_context.RecordManager.CurrentEntry != null)
        {
            _context.RecordManager.CurrentEntry.CarryCleared = true;
        }
    }
}
