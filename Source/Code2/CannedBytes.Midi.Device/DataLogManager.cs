using System;
using System.Collections.Generic;
using System.Text;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

public sealed partial class DataLogManager
{
    private DeviceDataContext _context;

    public DataLogManager(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        _context = context;
    }

    private List<DataLogEntry> _entries = new();

    public IEnumerable<DataLogEntry> Entries
    {
        get { return _entries; }
    }

    private readonly Stack<DataLogEntry> _currentEntries = new();

    public DataLogEntry? CurrentEntry
    {
        get
        {
            if (_currentEntries.Count > 0)
            {
                return _currentEntries.Peek();
            }

            return null;
        }
    }

    public IDisposable BeginNewEntry()
    {
        DataLogEntry entry = new()
        {
            Node = _context.FieldInfo.CurrentNode,
            PhysicalStreamPosition = _context.StreamManager.PhysicalStream.Position,
            ParentStreamPosition = _context.StreamManager.CurrentStream.Position
        };

        _currentEntries.Push(entry);

        return new DataLogScope(this);
    }

    public void SaveCurrentEntry()
    {
        if (CurrentEntry is not null)
        {
            var entry = _currentEntries.Pop();

            _entries.Add(entry);
        }
    }

    public void CancelCurrentEntry()
    {
        if (_currentEntries.Count > 0)
        {
            _currentEntries.Pop();
        }
    }

    public void SetError(Exception e)
    {
        if (CurrentEntry is null)
        {
            DataLogEntry? errEntry = null;

            if (_entries.Count > 0)
            {
                var lastEntry = _entries[_entries.Count - 1];

                // there is no current entry, is the exception the same as logged in last entry?
                if (lastEntry.Error != e)
                {
                    // if not make a new entry
                    errEntry = lastEntry.Clone();
                    errEntry.AddMessage("Cloned from previous entry for new Error.");
                }
            }
            else
            {
                errEntry = new DataLogEntry();
                errEntry.AddMessage("Auto-created new entry for Error.");
            }

            if (errEntry is not null)
            {
                errEntry.Error = e;
                _entries.Add(errEntry);
            }
            else
            {
                // now what?
                throw e;
            }
        }
        else
        {
            CurrentEntry.Error = e;
        }
    }

    public override string ToString()
    {
        var text = new StringBuilder();

        foreach (var entry in Entries)
        {
            text.AppendLine(entry.ToString());
        }

        return text.ToString();
    }
}
