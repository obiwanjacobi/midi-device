﻿using System;
using System.Collections.Generic;

namespace CannedBytes.Collections
{
    public class HierarchicalEnumerator<T> : DisposableBase, IEnumerable<T>, IEnumerator<T>
    {
        private readonly IEnumerator<T> _root;
        private readonly Stack<IEnumerator<T>> _enumStack = new Stack<IEnumerator<T>>();
        private IEnumerator<T> _currentEnum;

        public HierarchicalEnumerator(IEnumerable<T> root)
        {
            Check.IfArgumentNull(root, "root");

            _root = root.GetEnumerator();
            _enumStack.Push(_root);
        }

        public HierarchicalEnumerator(IEnumerator<T> root)
        {
            Check.IfArgumentNull(root, "root");

            _root = root;
            _enumStack.Push(_root);
        }

        public virtual T Current
        {
            get
            {
                ThrowIfDisposed();

                if (_currentEnum != null)
                {
                    return _currentEnum.Current;
                }

                throw new InvalidOperationException(
                    "A call to MoveNext() must return true before the Current property can be accessed.");
            }
        }

        public virtual bool MoveNext()
        {
            ThrowIfDisposed();

            bool hasMore = false;

            if (_currentEnum != null)
            {
                T current = _currentEnum.Current;

                if (current != null)
                {
                    var enumerator = GetChildEnumerator();

                    if (enumerator != null)
                    {
                        _enumStack.Push(_currentEnum);
                        _currentEnum = enumerator;
                    }
                }

                hasMore = _currentEnum.MoveNext();
            }

            // current enum is done, pop one from the stack
            while (hasMore == false &&
                _enumStack.Count > 0)
            {
                _currentEnum = GetParentEnumerator();

                if (_currentEnum != null)
                {
                    hasMore = _currentEnum.MoveNext();
                }
            }

            // reset vars when there's nothing more
            if (hasMore == false)
            {
                _currentEnum = null;
            }

            return hasMore;
        }

        public virtual void Reset()
        {
            ThrowIfDisposed();

            _currentEnum = null;
            _enumStack.Clear();
            _enumStack.Push(_root);
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        protected virtual IEnumerator<T> CurrentEnumerator
        {
            get { return _currentEnum; }
        }

        protected virtual IEnumerator<T> GetChildEnumerator()
        {
            var enumerable = Current as IEnumerable<T>;

            if (enumerable != null)
            {
                return enumerable.GetEnumerator();
            }

            var enumerator = Current as IEnumerator<T>;

            return enumerator;
        }

        protected virtual IEnumerator<T> GetParentEnumerator()
        {
            if (_enumStack.Count > 0)
            {
                return _enumStack.Pop();
            }

            return null;
        }

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            // nothing to do
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
