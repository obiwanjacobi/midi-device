namespace CannedBytes.Midi.Device.Schema
{
    public abstract class AttributedSchemaObject : SchemaObject
    {
        /// <summary>
        /// Default inheritance ctor.
        /// </summary>
        protected AttributedSchemaObject()
        {
        }

        /// <summary>
        /// Initializing inheritance ctor.
        /// </summary>
        protected AttributedSchemaObject(DeviceSchema schema, SchemaObjectName name)
            : base(schema, name)
        {
        }

        protected override void OnSchemaChanged()
        {
            base.OnSchemaChanged();

            if (_attributes != null)
            {
                _attributes.Schema = Schema;
            }
        }

        private SchemaAttributeCollection _attributes;

        public SchemaAttributeCollection Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    Attributes = new SchemaAttributeCollection();
                }

                return _attributes;
            }
            protected internal set
            {
                Check.IfArgumentNull(value, "Attributes");

                _attributes = value;
                _attributes.Schema = Schema;
            }
        }
    }
}