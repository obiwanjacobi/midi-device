using System.Collections.Generic;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Schema;

public sealed partial class FieldIterator : IEnumerable<FieldInfo>
{
    private readonly RecordType _root;
    private readonly int _repeat;

    public FieldIterator(RecordType root, int repeat)
    {
        Assert.IfArgumentNull(root, nameof(root));

        _root = root;
        _repeat = repeat;
    }

    public FieldIterator(Field field)
    {
        Assert.IfArgumentNull(field, nameof(field));
        Assert.IfArgumentNull(field.RecordType, "field.RecordType");

        _root = field.RecordType!;
        _repeat = field.Properties.Repeats;
    }

    public IEnumerator<FieldInfo> GetEnumerator()
    {
        if (_repeat > 1)
        {
            return new RepeatingFieldEnumerator(_root.Fields, _repeat);
        }

        return new FieldToFieldInfoEnumerator(_root.Fields);
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}