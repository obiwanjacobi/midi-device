using System;
using System.Collections.Generic;

namespace CannedBytes.Collections;

public class AggregateEnumerator<T> : DisposableBase, IEnumerable<T>, IEnumerator<T>
{
    private readonly List<IEnumerator<T>> _enumerators = new();
    private int _enumIndex = -1;
    private IEnumerator<T> _currentEnum;

    public int Count
    {
        get { return _enumerators.Count; }
    }

    public void Add(IEnumerable<T> iterator)
    {
        Check.IfArgumentNull(iterator, "iterator");
        ThrowIfDisposed();

        Add(iterator.GetEnumerator());
    }

    public void Add(IEnumerator<T> enumerator)
    {
        Check.IfArgumentNull(enumerator, "enumerator");
        ThrowIfDisposed();

        _enumerators.Add(enumerator);
    }

    public void Clear()
    {
        ThrowIfDisposed();

        _enumerators.Clear();
    }

    public void Remove(IEnumerator<T> enumerator)
    {
        Check.IfArgumentNull(enumerator, "enumerator");
        ThrowIfDisposed();

        _enumerators.Remove(enumerator);
    }

    public void ResetAll()
    {
        ThrowIfDisposed();

        foreach (IEnumerator<T> myEnum in _enumerators)
        {
            myEnum.Reset();
        }
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual T Current
    {
        get
        {
            ThrowIfDisposed();

            T val = default;

            if (_currentEnum != null)
            {
                val = _currentEnum.Current;
            }

            return val;
        }
    }

    object System.Collections.IEnumerator.Current
    {
        get { return Current; }
    }

    public virtual bool MoveNext()
    {
        ThrowIfDisposed();

        bool hasMore = false;

        _currentEnum ??= GetNextEnumerator();

        if (_currentEnum != null)
        {
            hasMore = _currentEnum.MoveNext();
        }

        while (_currentEnum != null && !hasMore)
        {
            _currentEnum = GetNextEnumerator();

            if (_currentEnum != null)
            {
                hasMore = _currentEnum.MoveNext();
            }
        }

        return hasMore;
    }

    public virtual void Reset()
    {
        ThrowIfDisposed();

        ResetAll();

        _enumIndex = -1;
        _currentEnum = null;
    }

    protected virtual IEnumerator<T> GetNextEnumerator()
    {
        ThrowIfDisposed();

        IEnumerator<T> myEnum = null;

        _enumIndex++;

        if (_enumIndex > -1 && _enumIndex < Count)
        {
            myEnum = _enumerators[_enumIndex];
        }
        else
        {
            _enumIndex--;
        }

        return myEnum;
    }

    protected override void Dispose(DisposeObjectKind disposeKind)
    {
        if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
        {
            foreach (IDisposable item in _enumerators)
            {
                item.Dispose();
            }

            _enumerators.Clear();
        }
    }
}
