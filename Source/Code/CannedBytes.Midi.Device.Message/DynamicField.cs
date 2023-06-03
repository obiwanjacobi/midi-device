using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class DynamicField : Field
    {
        public DynamicField(string fullName, DataType dataType)
            : base(fullName)
        {
            DataType = dataType;

            this.Attributes = dataType.Attributes;
            this.Constraints = dataType.Constraints;
            this.Repeats = 1;
        }

        public DynamicField(Field originalField)
            : base(originalField.Name.FullName)
        {
            this.Attributes = originalField.Attributes;
            this.Constraints = originalField.Constraints;
            if (originalField.DataType != null)
            {
                this.DataType = originalField.DataType;
            }

            if (originalField.DeclaringRecord != null)
            {
                this.DeclaringRecord = new DynamicRecordType(originalField.DeclaringRecord);
            }

            this.IsAbstract = originalField.IsAbstract;
            //this.MaxOccurs = originalField.MaxOccurs;
            //this.MinOccurs = originalField.MinOccurs;
            this.Repeats = 1;
            // this.Name -> set in constructor
            this.OriginalField = originalField;
            if (originalField.RecordType != null)
            {
                this.RecordType = new DynamicRecordType(originalField.RecordType);
            }
            if (originalField.Schema != null)
            {
                this.Schema = originalField.Schema;
            }
        }

        public void Stamp(int address, int size)
        {
            DynamicRecordType.Stamp(address, size);
            DynamicDeclaringRecord.Stamp(address, size);
        }

        public Field OriginalField { get; protected set; }

        public DynamicRecordType DynamicDeclaringRecord
        {
            get { return (DynamicRecordType)this.DeclaringRecord; }
            set { this.DeclaringRecord = value; }
        }

        public DynamicRecordType DynamicRecordType
        {
            get { return (DynamicRecordType)this.RecordType; }
            set { this.RecordType = value; }
        }
    }
}