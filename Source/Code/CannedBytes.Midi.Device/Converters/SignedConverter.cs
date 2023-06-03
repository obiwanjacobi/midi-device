using System;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    public partial class SignedConverter : ConverterExtension
    {
        public SignedConverter(DataType dataType)
            : base(dataType)
        {
            var constraint = dataType.FindConstraint(ConstraintType.FixedLength);

            if (constraint != null)
            {
                _byteLength = constraint.GetValue<int>();
            }
        }

        private int _byteLength;

        public override int ByteLength
        {
            get { return _byteLength; }
        }

        protected override IConverterProcess CreateProcess(MidiDeviceDataContext context)
        {
            IConverterProcess process = null;
            var valueOffset = DataType.ValueOffset;

            switch (_byteLength)
            {
                case 0:
                    if (this.InnerConverter != null)
                    {
                        // carry uses UInt16
                        process = new Int16Process(context, valueOffset);
                    }
                    else
                    {
                        throw new NotSupportedException("No FixedLength Constraint was specified.");
                    }
                    break;
                case 2:
                    process = new Int16Process(context, valueOffset);
                    break;
                case 4:
                    process = new Int32Process(context, valueOffset);
                    break;
                case 8:
                    process = new Int64Process(context, valueOffset);
                    break;
                default:
                    throw new NotSupportedException("The specified byte length " + _byteLength + " is not supported.");
            }

            return process;
        }
    }
}