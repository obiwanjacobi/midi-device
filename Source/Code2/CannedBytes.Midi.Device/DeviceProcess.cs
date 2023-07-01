using System;

namespace CannedBytes.Midi.Device;

public abstract class DeviceProcess<T> where T : DeviceDataContext
{
    protected DeviceProcess(T context, SchemaNode rootNode)
    {
        Context = context;
        RootNode = rootNode;
    }

    protected T Context { get; }

    public SchemaNode RootNode { get; }

    public SchemaNode? CurrentNode
    {
        get { return Context.FieldInfo.CurrentNode; }
        protected set { Context.FieldInfo.CurrentNode = value; }
    }

    protected void Execute(SchemaNode thisNode)
    {
        if (thisNode.IsRecord)
        {
            ProcessRecord(thisNode);
        }
        else
        {
            ProcessField(thisNode);
        }
    }

    private void ProcessRecord(SchemaNode thisNode)
    {
        using IDisposable scope = Context.LogManager.BeginNewEntry();
        try
        {
            OnBeforeRecord(thisNode);
            OnRecord(thisNode);
            OnAfterRecord(thisNode);
        }
        catch (Exception e)
        {
            Context.LogManager.SetError(e);

            throw;
        }
    }

    private void ProcessField(SchemaNode thisNode)
    {
        using IDisposable scope = Context.LogManager.BeginNewEntry();
        try
        {
            OnBeforeField(thisNode);
            OnField(thisNode);
            OnAfterField(thisNode);
        }
        catch (Exception e)
        {
            Context.LogManager.SetError(e);

            throw;
        }
    }

    protected virtual void OnBeforeRecord(SchemaNode thisNode)
    {
        CurrentNode = thisNode;

        var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnBeforeRecord(Context);
    }

    protected virtual void OnRecord(SchemaNode thisNode)
    {
        var iterator = thisNode.FieldConverterPair.StreamConverter!.GetChildNodeIterator(Context);

        foreach (SchemaNode node in iterator)
        {
            Execute(node);
        }
    }

    protected virtual void OnAfterRecord(SchemaNode thisNode)
    {
        var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnAfterRecord(Context);
    }

    private void OnBeforeField(SchemaNode thisNode)
    {
        CurrentNode = thisNode;
        var navEvents = thisNode.Parent?.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnBeforeField(Context);
    }

    protected abstract void OnField(SchemaNode thisNode);

    private void OnAfterField(SchemaNode thisNode)
    {
        var navEvents = thisNode.Parent?.FieldConverterPair.StreamConverter as INavigationEvents;

        navEvents?.OnAfterField(Context);
    }

    protected void LogCarryCleared()
    {
        if (Context.LogManager.CurrentEntry is not null)
        {
            Context.LogManager.CurrentEntry.CarryCleared = true;
        }
    }
}
