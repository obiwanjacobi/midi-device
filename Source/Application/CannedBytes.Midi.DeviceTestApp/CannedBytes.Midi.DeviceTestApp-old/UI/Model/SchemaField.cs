using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.DeviceTestApp.UI.Model
{
    class SchemaField
    {
        public SchemaField(Field field)
        {
            this.Field = field;
        }

        public SchemaField(SchemaField parent, Field field)
        {
            this.Parent = parent;
            this.Field = field;
        }

        public Field Field { get; private set; }

        public SchemaField Parent { get; set; }

        public int InstanceIndex { get; set; }

        public string RepeatsFormatted
        {
            get
            {
                if (this.Field.Repeats > 1)
                {
                    return String.Format("({0})", this.Field.Repeats);
                }

                return string.Empty;
            }
        }

        public IEnumerable<int> IndexValues
        {
            get
            {
                for (int i = 0; i < Field.Repeats; i++)
                {
                    yield return i;
                }
            }
        }

        public IEnumerable<SchemaField> ParentFields
        {
            get
            {
                var parent = this;

                while (parent != null)
                {
                    yield return parent;

                    parent = parent.Parent;
                }
            }
        }

        public IEnumerable<SchemaField> MultiFields
        {
            get
            {
                var parent = this;

                while (parent != null)
                {
                    if (parent.Field.Repeats > 1)
                    {
                        yield return parent;
                    }

                    parent = parent.Parent;
                }
            }
        }

        public string Name
        {
            get { return this.Field.Name.Name; }
        }

        public string SchemaName
        {
            get { return this.Field.Name.SchemaName; }
        }

        public string Type
        {
            get
            {
                if (this.Field.DataType != null)
                {
                    return this.Field.DataType.Name.Name;
                }

                if (this.Field.RecordType != null)
                {
                    return this.Field.RecordType.Name.Name;
                }

                return String.Empty;
            }
        }

        public IEnumerable<SchemaField> Fields
        {
            get
            {
                // TODO: derived record type's fields.
                if (this.Field.RecordType != null)
                {
                    if (this.Field.RecordType.BaseType != null)
                    {
                        return from field in this.Field.RecordType.BaseType.Fields.Union(this.Field.RecordType.Fields)
                               select new SchemaField(this, field);
                    }

                    return from field in this.Field.RecordType.Fields
                           select new SchemaField(this, field);
                }

                return new List<SchemaField>();
            }
        }
    }
}