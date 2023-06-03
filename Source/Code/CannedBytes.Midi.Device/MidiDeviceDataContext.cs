namespace CannedBytes.Midi.Device
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using CannedBytes.Midi.Device.Converters;
    using CannedBytes.Midi.Device.Schema;

    public class MidiDeviceDataContext //: ServiceContainer
    {
        public MidiDeviceDataContext(FieldConverterPair rootPair)
            : this((GroupConverter)rootPair.Converter)
        {
            this.CurrentFieldConverter = rootPair;
        }

        public MidiDeviceDataContext(GroupConverter rootConverter)
            : this(rootConverter.RecordType, rootConverter)
        {
        }

        public MidiDeviceDataContext(RecordType recordType, GroupConverter rootConverter)
        {
            Check.IfArgumentNull(recordType, "recordType");
            Check.IfArgumentNull(rootConverter, "rootConverter");
            if (recordType.Name.FullName != rootConverter.RecordType.Name.FullName)
            {
                throw new ArgumentException("The specified recordType and rootConverter do not match up.", "rooConverter");
            }

            _recordType = recordType;
            _rootConverter = rootConverter;
        }

        private DevicePropertyCollection _deviceProperties;

        /// <summary>
        /// Gets a collection of device properties that were found during the to logical transformation.
        /// </summary>
        public DevicePropertyCollection DeviceProperties
        {
            get
            {
                if (this._deviceProperties == null)
                {
                    DeviceProperties = new DevicePropertyCollection();
                }

                return this._deviceProperties;
            }
            set
            {
                Check.IfArgumentNull(value, "DeviceProperties");
                this._deviceProperties = value;
            }
        }

        /// <summary>
        /// Creates a context object to be used for calls to the logical application component.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public virtual MidiLogicalContext CreateLogicalContext()
        {
            return new MidiLogicalContext(RecordType, CurrentFieldConverter.Field, EnumeratorStack);
        }

        /// <summary>
        /// Gets or set the composition container that can be used to retrieve instance of registered types.
        /// </summary>
        public CompositionContainer CompositionContainer { get; set; }

        private bool _forceLogicCall;

        /// <summary>
        /// Gets or sets a value indicating whether to always call the logical reader or writer
        /// even when the Converter/DataType would otherwise not (true).
        /// </summary>
        public bool ForceLogicCall
        {
            get { return _forceLogicCall; }
            set { _forceLogicCall = value; }
        }

        private Carry _carry = new Carry();

        /// <summary>
        /// Gets the current bit carry for physical stream operations.
        /// </summary>
        public Carry Carry
        {
            get { return this._carry; }
        }

        private SysExStream physicalStream;

        /// <summary>
        /// Gets or sets the physical stream.
        /// </summary>
        /// <remarks>Only converters that need access to the physical stream
        /// (like checksum) use this property. Otherwise use <see cref="CurrentStream"/>.</remarks>
        public Stream PhysicalStream
        {
            get { return this.physicalStream; }
            set
            {
                if (value != null)
                {
                    this.physicalStream = value as SysExStream;

                    if (this.physicalStream == null)
                    {
                        this.physicalStream = new SysExStream(value);
                    }
                }
            }
        }

        private Stack<Stream> streamStack = new Stack<Stream>();

        /// <summary>
        /// The current stream is used for reading and writing.
        /// (Group) converters can insert their own streams to be used.
        /// </summary>
        /// <remarks>Assign null to remove set stream and restore previous.</remarks>
        public Stream CurrentStream
        {
            get
            {
                if (this.streamStack.Count == 0)
                {
                    return PhysicalStream;
                }

                return this.streamStack.Peek();
            }

            set
            {
                if (value != null)
                {
                    this.streamStack.Push(value);
                }
                else if (this.streamStack.Count > 0)
                {
                    this.streamStack.Pop();
                }
            }
        }

        private MidiDeviceDataRecordList _records;

        public MidiDeviceDataRecordList DataRecords
        {
            get
            {
                if (_records == null)
                {
                    _records = new MidiDeviceDataRecordList();
                }

                return _records;
            }
        }

        private RecordType _recordType;

        public RecordType RecordType
        {
            get { return _recordType; }
        }

        public int CurrentInstanceIndex
        {
            get
            {
                if (EnumeratorStack.Count > 0)
                {
                    return EnumeratorStack.Peek().InstanceIndex;
                }

                return 0;
            }
        }

        public void Reset()
        {
            _carry.Set(0, BitFlags.None);
            if (_deviceProperties != null)
            {
                _deviceProperties.Clear();
            }
            EnumeratorStack.Clear();
            if (_records != null)
            {
                _records.Clear();
            }
            streamStack.Clear();
        }

        private FieldConverterPair _currentFieldConverter;

        public FieldConverterPair CurrentFieldConverter
        {
            get
            {
                if (EnumeratorStack.Count > 0)
                {
                    return EnumeratorStack.Peek().Current;
                }

                return _currentFieldConverter;
            }
            protected set { _currentFieldConverter = value; }
        }

        private GroupConverter _rootConverter;

        public GroupConverter RootConverter
        {
            get { return _rootConverter; }
        }

        public GroupConverter CurrentParentConverter
        {
            get
            {
                if (EnumeratorStack.Count > 0)
                {
                    return EnumeratorStack.Peek().ParentConverter;
                }

                return _rootConverter;
            }
        }

        private Stack<FieldConverterEnumerator> _enumStack;

        protected Stack<FieldConverterEnumerator> EnumeratorStack
        {
            get
            {
                if (_enumStack == null)
                {
                    _enumStack = new Stack<FieldConverterEnumerator>();
                }

                return _enumStack;
            }

            set
            {
                _enumStack = value;
            }
        }

        public virtual FieldConverterEnumerator GetEnumerator(GroupConverter parentConverter)
        {
            FieldConverterEnumerator enumerator = CreateEnumerator(parentConverter);

            EnumeratorStack.Push(enumerator);

            return enumerator;
        }

        protected virtual FieldConverterEnumerator CreateEnumerator(GroupConverter parentConverter)
        {
            return new FieldConverterEnumerator(parentConverter, this);
        }

        /// <summary>
        /// Don't call this method, instead call Dispose on the Enumerator object.
        /// </summary>
        /// <param name="enumerator">Must not be null.</param>
        public void RemoveEnumerator(FieldConverterEnumerator enumerator)
        {
            FieldConverterEnumerator popped = EnumeratorStack.Pop();

            if (enumerator != popped)
            {
                throw new MidiDeviceDataException("Field-GroupConverter mismatch.");
            }
        }

        public virtual void ToLogical(IMidiLogicalWriter writer)
        {
            if (_rootConverter != null)
            {
                // inject the properties writer
                var propertiesWriter = new MidiDevicePropertiesLogicalWriter(this, writer);
                _rootConverter.ToLogical(this, propertiesWriter);
            }
        }

        public virtual void ToPhysical(IMidiLogicalReader reader)
        {
            if (_rootConverter != null)
            {
                // inject properties reader
                var propertiesReader = new MidiDevicePropertiesLogicalReader(this, reader);
                _rootConverter.ToPhysical(this, propertiesReader);

                // first flush the carry then the writer
                Carry.Flush(CurrentStream);
                CurrentStream.Flush();

                // terminate sysex message
                physicalStream.WriteEndMarker();
            }
        }
    }
}