namespace CannedBytes.Midi.Device
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using CannedBytes.Midi.Device.Schema;

    public class MidiLogicalStreamReader
    {
        public MidiLogicalStreamReader(IMidiLogicalReader reader)
        {
            _reader = reader;
        }

        private IMidiLogicalReader _reader;

        public IMidiLogicalReader MidiLogicalReader
        {
            get { return _reader; }
        }

        public byte[] Read(MidiLogicalContext context)
        {
            Type targetType = GetNativeType(context.Field);

            if (targetType == null)
            {
                throw new MidiDeviceDataException(
                    "Could not resolve the Data Type for Field: " + context.Field.Name.FullName);
            }

            int sizeInBytes = Marshal.SizeOf(targetType);
            byte[] buffer = null;

            if (targetType == typeof(bool))
            {
                bool value = MidiLogicalReader.ReadBool(context);

                buffer = BitConverter.GetBytes(value);
            }

            if (targetType == typeof(byte))
            {
                byte value = MidiLogicalReader.ReadByte(context);

                buffer = new byte[] { value };
            }

            if (targetType == typeof(Int32))
            {
                Int32 value = MidiLogicalReader.ReadInt32(context);

                buffer = BitConverter.GetBytes(value);
            }

            if (targetType == typeof(Int64))
            {
                Int64 value = MidiLogicalReader.ReadInt64(context);

                buffer = BitConverter.GetBytes(value);
            }

            if (targetType == typeof(string))
            {
                string value = MidiLogicalReader.ReadString(context);

                buffer = Encoding.ASCII.GetBytes(value);
            }

            return buffer;
        }

        public static Type GetNativeType(Field field)
        {
            if (field != null && field.DataType != null)
            {
                return GetNativeType(field.DataType);
            }

            return null;
        }

        public static Type GetNativeType(DataType dataType)
        {
            switch (dataType.Name.Name)
            {
                case "midiBit0":
                case "midiBit1":
                case "midiBit2":
                case "midiBit3":
                case "midiBit4":
                case "midiBit5":
                case "midiBit6":
                case "midiBit7":
                case "midiBit8":
                case "midiBit9":
                case "midiBit10":
                case "midiBit11":
                case "midiBit12":
                case "midiBit13":
                case "midiBit14":
                case "midiBit15":
                    return typeof(bool);

                case "midiByte":
                    return typeof(byte);

                default:
                    if (dataType.BaseType != null)
                    {
                        return GetNativeType(dataType.BaseType);
                    }
                    break;
            }

            return null;
        }
    }
}