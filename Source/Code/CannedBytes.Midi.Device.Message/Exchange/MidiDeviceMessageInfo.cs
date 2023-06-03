using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// The MidiDeviceMessageInfo structure contains information about the structure
    /// and content of a Midi SysEx message.
    /// </summary>
    public class MidiDeviceMessageInfo
    {
        public MidiDeviceMessageInfo(RecordType envelopeType, RecordType bodyType,
            int manufacturerId, int modelId, byte sysexChannel)
        {
            _envelopeType = envelopeType;
            _bodyRecordType = bodyType;
            _manufacturerId = manufacturerId;
            _modelId = modelId;
            _sysexChannel = sysexChannel;
        }

        private RecordType _envelopeType;
        /// <summary>
        /// The <see cref="RecordType"/> of the message Envelope.
        /// </summary>
        public RecordType EnvelopeRecordType
        {
            get { return _envelopeType; }
        }

        private RecordType _bodyRecordType;
        /// <summary>
        /// The <see cref="RecordType"/> of the message Body.
        /// </summary>
        public RecordType BodyDataRecordType
        {
            get { return _bodyRecordType; }
        }

        private int _manufacturerId;
        /// <summary>
        /// The manufacturer ID of the device.
        /// </summary>
        public int ManufacturerId
        {
            get { return _manufacturerId; }
        }

        private int _modelId;
        /// <summary>
        /// The type of device (model).
        /// </summary>
        public int ModelId
        {
            get { return _modelId; }
        }

        private byte _sysexChannel;
        /// <summary>
        /// A device instance identifier -or- SysEx Channel.
        /// </summary>
        public byte SysExChannel
        {
            get { return _sysexChannel; }
        }
    }
}