namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using System.IO;
    using CannedBytes.IO;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The GroupConverter class implements a basic group converter without any additional (special) logic.
    /// </summary>
    public class GroupConverter : IConverter
    {
        /// <summary>
        /// Constructs an instance based on the specified <paramref name="recordType"/>.
        /// </summary>
        /// <param name="recordType">Must not be null.</param>
        public GroupConverter(RecordType recordType)
        {
            Check.IfArgumentNull(recordType, "recordType");

            _recordType = recordType;
        }

        private RecordType _recordType;

        /// <summary>
        /// Gets the record type this group converter represents.
        /// </summary>
        public RecordType RecordType
        {
            get { return _recordType; }
        }

        /// <summary>
        /// Gets an indication if the contents of this converter is dynamic.
        /// </summary>
        /// <remarks>Used for address mapped messaging.</remarks>
        public bool IsDynamic { get; protected set; }

        /// <summary>
        /// Gets an indication if there are child field converters present.
        /// </summary>
        public bool HasConverters
        {
            get { return (_fieldConverterMap != null && _fieldConverterMap.Count > 0); }
        }

        private FieldConverterMap _fieldConverterMap;

        /// <summary>
        /// Gets the map that relates fields to converters.
        /// </summary>
        /// <remarks>Derived classes can also set this value but can only do so before first access.</remarks>
        public FieldConverterMap FieldConverterMap
        {
            get
            {
                if (_fieldConverterMap == null)
                {
                    FieldConverterMap = new FieldConverterMap();
                }

                return _fieldConverterMap;
            }
            protected set
            {
                Check.IfArgumentNull(value, "FieldConverterMap");

                _fieldConverterMap = value;
                _byteLength = 0;
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get { return RecordType.Name.Name; }
        }

        private int _byteLength;

        /// <inheritdoc/>
        public virtual int ByteLength
        {
            get
            {
                if (_byteLength == 0)
                {
                    var carry = new Carry();
                    foreach (var pair in FieldConverterMap)
                    {
                        _byteLength += CalculateByteLength(pair.Converter, carry);
                    }
                }

                return _byteLength;
            }
        }

        public virtual int CalculateByteLength(IConverter converter, Carry carry)
        {
            int byteLength = converter.ByteLength;

            if (byteLength < 0)
            {
                ushort temp = 0;
                var flags = (BitFlags)Math.Abs(byteLength);

                if (carry != null)
                {
                    byteLength = carry.ReadFrom(null, flags, out temp);
                }
                else
                {
                    throw new ArgumentException("You have to provide a Carry to calculate bit fields.", "carry");
                }
            }
            else if (carry != null)
            {
                // clear
                carry.Set(0, BitFlags.None);
            }

            return byteLength;
        }

        /// <inheritdoc/>
        public virtual void ToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            // start of record clears Carry
            context.Carry.Clear();

            context.CurrentStream = BuildStream(context.PhysicalStream);

            try
            {
                RecordToLogical(context, writer);
            }
            finally
            {
                context.CurrentStream = null;
            }

            // end of record clears Carry
            context.Carry.Clear();
        }

        /// <inheritdoc/>
        public virtual void ToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            // start of a record starts with a new Carry
            context.Carry.Flush(context.CurrentStream);

            context.CurrentStream = BuildStream(context.PhysicalStream);

            try
            {
                RecordToPhysical(context, reader);

                // make sure all bytes have been written to stream this converter setup.
                context.Carry.Flush(context.CurrentStream);
                context.CurrentStream.Flush();
            }
            finally
            {
                context.CurrentStream = null;
            }
        }

        protected virtual void RecordToLogical(MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            if (HasConverters)
            {
                using (FieldConverterEnumerator enumerator = context.GetEnumerator(this))
                {
                    while (enumerator.MoveNext())
                    {
                        FieldToLogical(enumerator.Current, context, writer);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected virtual void RecordToPhysical(MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            if (HasConverters)
            {
                using (FieldConverterEnumerator enumerator = context.GetEnumerator(this))
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Converter.ToPhysical(context, reader);
                    }
                }
            }
        }

        protected virtual void FieldToLogical(FieldConverterPair pair, MidiDeviceDataContext context, IMidiLogicalWriter writer)
        {
            pair.Converter.ToLogical(context, writer);
        }

        protected virtual void FieldToPhysical(FieldConverterPair pair, MidiDeviceDataContext context, IMidiLogicalReader reader)
        {
            pair.Converter.ToPhysical(context, reader);
        }

        protected virtual Stream BuildStream(Stream stream)
        {
            if (stream.Length > 0)
            {
                return new SubStream(stream, stream.Length - stream.Position);
            }
            else
            {
                return new SubStream(stream, ByteLength);
            }
        }
    }
}