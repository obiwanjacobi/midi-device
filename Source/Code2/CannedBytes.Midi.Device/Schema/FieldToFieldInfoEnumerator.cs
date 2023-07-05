using System;
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool dispose)
    {
        _enum.Dispose();
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

    object System.Collections.IEnumerator.Current
    {
        get { return Current; }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this;
    }
}
