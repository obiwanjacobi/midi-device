using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema;

public sealed partial class FieldHierarchicalIterator : IEnumerable<FieldInfo>
{
    private readonly RecordType _rootRecord;

    public FieldHierarchicalIterator(RecordType rootRecord)
    {
        Check.IfArgumentNull(rootRecord, "rootRecord");

        _rootRecord = rootRecord;
    }

    public bool ExpandRepeatingFields { get; set; }

    // enumerates repeated fields
    public IEnumerator<FieldInfo> GetEnumerator()
    {
        if (ExpandRepeatingFields)
        {
            return new RepeatingFieldHierarchicalEnumerator(ToFieldInfos(_rootRecord.Fields));
        }

        return new FieldHierarchicalEnumerator(ToFieldInfos(_rootRecord.Fields));
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal static IEnumerable<FieldInfo> ToFieldInfos(IEnumerable<Field> fields)
    {
        return new FieldToFieldInfoEnumerator(fields);
    }
}
