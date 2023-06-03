using System;

using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class DynamicRecordType : RecordType
    {
        public DynamicRecordType(RecordType originalType)
            : base(originalType.Name.FullName)
        {
            CopyFrom(originalType);
        }

        public void Stamp(int address, int size)
        {
            string postfix = String.Format("_[{0}:{1}]", address, size);

            this.Name = new SchemaObjectName(this.Name.FullName + postfix);
        }

        private void CopyFrom(RecordType recordType)
        {
            this.Attributes = recordType.Attributes;
            if (recordType.BaseType != null)
            {
                this.BaseType = recordType.BaseType;
            }
            this.Fields = new DuplicateFieldCollection();
            //this.FlattenedFields not used.
            this.IsAbstract = recordType.IsAbstract;
            this.IsDynamic = true;
            this.OriginalType = recordType;
            this.Schema = recordType.Schema;
        }

        public RecordType OriginalType { get; protected set; }

        public DynamicField AddField(Field originalField)
        {
            var field = originalField as DynamicField;

            if (field == null)
            {
                field = new DynamicField(originalField);
            }

            field.DynamicDeclaringRecord = this;

            this.Fields.Add(field);

            return field;
        }

        //---------------------------------------------------------------------

        private class DuplicateFieldCollection : FieldCollection
        {
            protected override string GetKeyForItem(Field item)
            {
                // put a limit to it
                for (int index = 1; index < 65535; index++)
                {
                    string key = String.Format("{0}_[{1}]", base.GetKeyForItem(item), index);

                    if (!base.Contains(key))
                    {
                        return key;
                    }
                }

                return null;
            }
        }
    }
}