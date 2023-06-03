namespace CannedBytes.Midi.Device
{
    using System.ComponentModel.Composition;
    using CannedBytes.Midi.Device.Converters;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The default generic device provider.
    /// </summary>
    [DeviceProvider(typeof(MidiDeviceProvider), "", "", 0, 0)]
    public class MidiDeviceProvider : IMidiDeviceProvider, IPartImportsSatisfiedNotification
    {
        [Import(typeof(IDeviceSchemaProvider))]
        protected IDeviceSchemaProvider SchemaProvider { get; private set; }

        [Import(typeof(ConverterManager))]
        protected ConverterManager ConverterManager { get; private set; }

        public string Manufacturer { get; protected set; }

        public string ModelName { get; protected set; }

        public byte ManufacturerId { get; protected set; }

        public byte ModelId { get; protected set; }

        public byte DeviceId { get; set; }

        public DeviceSchema Schema { get; protected set; }

        public FieldConverterMap RootTypes { get; protected set; }

        public bool IsMessageProvider { get; protected set; }

        protected string SchemaName { get; set; }

        public virtual void Initialze(string schema, string manufacturer, string model, byte manId, byte modId)
        {
            this.SchemaName = schema;
            this.Manufacturer = manufacturer;
            this.ManufacturerId = manId;
            this.ModelName = model;
            this.ModelId = modId;

            Initialize();
        }

        protected virtual void InitializeSchema()
        {
            if (!string.IsNullOrEmpty(this.SchemaName))
            {
                Schema = SchemaProvider.Load(SchemaName);
            }
        }

        protected virtual void InitializeRootConverters()
        {
            if (this.Schema != null)
            {
                this.RootTypes = new FieldConverterMap();

                foreach (var rootType in Schema.RootRecordTypes)
                {
                    var converter = this.ConverterManager.GetConverter(rootType);
                    var field = new Field(rootType.Name.FullName);
                    var pair = new FieldConverterPair(field, converter);

                    this.RootTypes.Add(pair);
                }
            }
        }

        protected bool InitializePropertiesFromDeviceProviderAttribute()
        {
            var attr = DeviceProviderAttribute.From(this.GetType());

            if (attr != null)
            {
                Manufacturer = attr.Manufacturer;
                ManufacturerId = attr.ManufacturerId;
                ModelName = attr.ModelName;
                ModelId = attr.ModelId;

                return true;
            }

            return false;
        }

        protected virtual void Initialize()
        {
            InitializeSchema();
            InitializeRootConverters();
        }

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            Initialize();
        }
    }
}