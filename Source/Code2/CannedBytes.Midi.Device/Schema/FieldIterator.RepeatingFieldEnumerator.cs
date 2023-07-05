using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema;

partial class FieldIterator
{
    private sealed class RepeatingFieldEnumerator : FieldToFieldInfoEnumerator
    {
        private readonly int _repeat;
        private int _instanceIndex;

        public RepeatingFieldEnumerator(IEnumerable<Field> fields, int repeat)
            : base(fields)
        {
            _repeat = repeat;
            _instanceIndex = 0;
        }

        public override FieldInfo Current
        {
            get
            {
                var current = base.Current;
                current.InstanceIndex = _instanceIndex;

                return current;
            }
        }

        public override bool MoveNext()
        {
            var hasMore = base.MoveNext();

            if (!hasMore &&
                _instanceIndex < _repeat -1 &&
                _repeat > 1)
            {
                _instanceIndex++;
                base.Reset();

                hasMore = base.MoveNext();
            }

            return hasMore;
        }

        public override void Reset()
        {
            _instanceIndex = 0;
            base.Reset();
        }
    }
}
