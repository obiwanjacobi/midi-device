namespace CannedBytes.Midi.Device.Schema
{
    using System;

    /// <summary>
    /// Manages the schema (namespace) and object name.
    /// </summary>
    public class SchemaObjectName
    {
        private const char SchemaNameSeparator = ':';

        protected SchemaObjectName()
        { }

        public SchemaObjectName(string fullName)
        {
            int index = fullName.LastIndexOf(SchemaNameSeparator);
            if (index < 0) throw new ArgumentException("Cannot parse fullName: " + fullName);

            _name = fullName.Substring(index + 1);
            _schemaName = fullName.Substring(0, index);
            _fullName = fullName;
        }

        public SchemaObjectName(string schemaName, string objectName)
        {
            _schemaName = schemaName;
            _name = objectName;
            _fullName = schemaName + SchemaNameSeparator + objectName;
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        private string _schemaName;

        public string SchemaName
        {
            get { return _schemaName; }
            protected set { _schemaName = value; }
        }

        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            protected set { _fullName = value; }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}