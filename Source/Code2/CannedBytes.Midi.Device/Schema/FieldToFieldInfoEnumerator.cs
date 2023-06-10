using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema;

internal class FieldToFieldInfoEnumerator : IEnumerator<FieldInfo>, IEnumerable<FieldInfo>
{
    private readonly IEnumerator<Field> _enum;

    public FieldToFieldInfoEnumerator(IEnumerable<Field> iterator)
    {
        _enum = iterator.GetEnumerator();
    }

    public FieldToFieldInfoEnumerator(IEnumerator<Field> enumerator)
    {
        _enum = enumerator;
    }

    public virtual FieldInfo Current
    {
        get { return new FieldInfo(_enum.Current); }
    }

    public virtual void Dispose()
    {
        _enum.Dispose();
    }

    object System.Collections.IEnumerator.Current
    {
        get { return Current; }
    }

    public virtual bool MoveNext()
    {
        return _enum.MoveNext();
    }

    public virtual void Reset()
    {
        _enum.Reset();
    }

    public IEnumerator<FieldInfo> GetEnumerator()
    {
        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
