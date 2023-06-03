namespace CannedBytes.Midi.Device
{
    using System.Collections.Generic;
    using CannedBytes.Midi.Device.Converters;

    public class FieldConverterEnumerator : DisposableBase, IEnumerator<FieldConverterPair>
    {
        private int _cycleCount;
        private MidiDeviceDataContext _context;
        private IEnumerator<FieldConverterPair> _enum;

        /// <summary>
        /// Constructs a new Enumerator instance, optionally tied the a <see cref="MidiDeviceDataContext"/>.
        /// </summary>
        /// <param name="parentConverter">Must not be null.</param>
        /// <param name="context">Can be null. If specified this instance is added to the
        /// <paramref name="context"/> and removed when Dispose is called.</param>
        public FieldConverterEnumerator(GroupConverter parentConverter, MidiDeviceDataContext context)
        {
            Check.IfArgumentNull(parentConverter, "parentConverter");
            Check.IfArgumentNull(context, "context");

            _context = context;
            _parentConverter = parentConverter;
            _enum = _parentConverter.FieldConverterMap.GetEnumerator();
            _parentConverter.FieldConverterMap.Locked = true;

            if (context.CurrentFieldConverter != null)
            {
                _cycleCount = context.CurrentFieldConverter.Field.Repeats;
            }
            else
            {
                _cycleCount = 1;
            }
        }

        public FieldConverterEnumerator(GroupConverter parentConverter, int maxInstanceIndex)
        {
            Check.IfArgumentNull(parentConverter, "parentConverter");

            _parentConverter = parentConverter;
            _enum = _parentConverter.FieldConverterMap.GetEnumerator();
            _parentConverter.FieldConverterMap.Locked = true;
            _cycleCount = maxInstanceIndex;
        }

        private GroupConverter _parentConverter;

        public GroupConverter ParentConverter
        {
            get { return _parentConverter; }
            protected set { _parentConverter = value; }
        }

        public bool IsFirst { get; protected set; }

        private int _instanceIndex;

        public int InstanceIndex
        {
            get { return _instanceIndex; }
            protected set { _instanceIndex = value; }
        }

        #region IEnumerator<FieldConverterPair> Members

        public virtual FieldConverterPair Current
        {
            get { return _enum.Current; }
        }

        #endregion IEnumerator<FieldConverterPair> Members

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public virtual bool MoveNext()
        {
            bool continueEnum = _enum.MoveNext();

            if (!continueEnum)
            {
                int cycleIndex = InstanceIndex + 1;

                if (cycleIndex < _cycleCount)
                {
                    _enum.Reset();

                    InstanceIndex = cycleIndex;

                    continueEnum = _enum.MoveNext();

                    IsFirst = continueEnum;
                }
            }
            else
            {
                IsFirst = false;
            }

            return continueEnum;
        }

        public virtual void Reset()
        {
            _enum.Reset();
        }

        #endregion IEnumerator Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            _parentConverter.FieldConverterMap.Locked = false;

            if (_context != null)
            {
                _context.RemoveEnumerator(this);
            }
        }
    }
}