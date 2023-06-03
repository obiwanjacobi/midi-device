namespace CannedBytes.Midi.Device
{
    using System.Collections.Generic;
    using System.Text;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// Manages a list of <see cref="MidiDeviceDataRecord"/> instances.
    /// </summary>
    public class MidiDeviceDataRecordList : List<MidiDeviceDataRecord>
    {
        /// <summary>
        /// Adds a new record to the list.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="data"></param>
        /// <param name="field"></param>
        /// <returns>Returns the record created.</returns>
        public MidiDeviceDataRecord Add(long pos, object data, Field field)
        {
            MidiDeviceDataRecord record =
                new MidiDeviceDataRecord(pos, field, data);

            base.Add(record);

            return record;
        }

        public MidiDeviceDataRecord Add(long pos, object data, Field field, bool carryFlushed)
        {
            MidiDeviceDataRecord record =
                new MidiDeviceDataRecord(pos, field, data, carryFlushed);

            base.Add(record);

            return record;
        }

        public override string ToString()
        {
            var text = new StringBuilder();

            foreach (var record in this)
            {
                text.AppendLine(record.ToString());
            }

            return text.ToString();
        }
    }
}