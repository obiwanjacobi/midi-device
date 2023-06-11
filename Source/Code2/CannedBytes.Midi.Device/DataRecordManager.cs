using System;
using System.Collections.Generic;
using System.Text;

namespace CannedBytes.Midi.Device;

public sealed partial class DataRecordManager
{
    private DeviceDataContext _context;

    public DataRecordManager(DeviceDataContext context)
    {
        Check.IfArgumentNull(context, nameof(context));

        _context = context;
    }

    private List<DataRecordEntry> _entries = new();

    public IEnumerable<DataRecordEntry> Entries
    {
        get { return _entries; }
    }

    private Stack<DataRecordEntry> _currentEntries = new();

    public DataRecordEntry CurrentEntry
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
        DataRecordEntry entry = new()
        {
            Node = _context.FieldInfo.CurrentNode,
            PhysicalStreamPosition = _context.StreamManager.PhysicalStream.Position,
            ParentStreamPosition = _context.StreamManager.CurrentStream.Position
        };

        _currentEntries.Push(entry);

        return new DataRecordScope(this);
    }

    public void SaveCurrentEntry()
    {
        if (CurrentEntry != null)
        {
            DataRecordEntry entry = _currentEntries.Pop();

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
        if (CurrentEntry == null)
        {
            DataRecordEntry errEntry = null;

            if (_entries.Count > 0)
            {
                DataRecordEntry lastEntry = _entries[_entries.Count - 1];

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
                errEntry = new DataRecordEntry();
                errEntry.AddMessage("Auto-created new entry for Error.");
            }

            if (errEntry != null)
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
        StringBuilder text = new();

        foreach (DataRecordEntry entry in Entries)
        {
            text.AppendLine(entry.ToString());
        }

        return text.ToString();
    }
}
