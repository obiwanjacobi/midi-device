using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema;

partial class FieldHierarchicalIterator
{
    private sealed class RepeatingFieldHierarchicalEnumerator : FieldHierarchicalEnumerator
    {
        private readonly Stack<int> _repeatStack = new();
        private int? _repeats;

        public RepeatingFieldHierarchicalEnumerator(IEnumerable<FieldInfo> fields)
            : base(fields)
        { }

        public override FieldInfo Current
        {
            get
            {
                var fieldInfo = base.Current;

                fieldInfo.InstanceIndex = _repeats.GetValueOrDefault();

                return fieldInfo;
            }
        }

        public override bool MoveNext()
        {
            var hasMore = base.MoveNext();

            if (hasMore)
            {
                if (Current.Field.Properties.Repeats > 1)
                {
                    _repeats = Current.Field.Properties.Repeats - 1;
                }
                else
                {
                    _repeats = 0;
                }
            }

            return hasMore;
        }

        public override void Reset()
        {
            base.Reset();

            _repeats = null;
            _repeatStack.Clear();
        }

        protected override IEnumerator<FieldInfo>? GetChildEnumerator()
        {
            var enumerator = base.GetChildEnumerator();

            if (enumerator is not null)
            {
                _repeatStack.Push(_repeats.GetValueOrDefault());
                _repeats = null;
            }

            return enumerator;
        }

        protected override IEnumerator<FieldInfo>? GetParentEnumerator()
        {
            if (_repeatStack.Count > 0)
            {
                _repeats = _repeatStack.Pop();
            }
            else
            {
                _repeats = null;
            }

            if (_repeats.HasValue && _repeats.Value > 0)
            {
                _repeats--;

                var enumerator = CurrentEnumerator;

                // do it again
                enumerator?.Reset();

                return enumerator;
            }

            return base.GetParentEnumerator();
        }
    }
}
