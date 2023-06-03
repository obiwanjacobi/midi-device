using System;
using System.IO;
using System.Linq;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    public class StdMessageProvider : IMessageProvider
    {
        private BinarySearchList searchList;

        public StdMessageProvider(DeviceSchema schema)
        {
            Schema = schema;
            this.searchList = new BinarySearchList(schema);
        }

        public DeviceSchema Schema { get; private set; }

        #region IMessageProvider Members

        public MidiDeviceMessageInfo GetMessageInfo(Stream physicalStream)
        {
            if (!physicalStream.CanSeek)
            {
                throw new ArgumentException("Stream must be seekable.", "physicalStream");
            }

            long repos = physicalStream.Position;

            int index = 0;
            byte[] streamBuffer = new byte[10];

            int bytesRead = physicalStream.Read(streamBuffer, index, 10);
            physicalStream.Position = repos;

            for (int n = 0; n < bytesRead; n++)
            {
                var result = this.searchList.Find(streamBuffer, bytesRead);

                if (result != null)
                {
                    GroupConverter envelopeType = result.FirstOrDefault();

                    // found exact match
                    if (envelopeType != null)
                    {
                        // TODO: determine body RecordType and ManufacturerID, ModelID and SysExChannel/DeviceID.
                        return new MidiDeviceMessageInfo(envelopeType.RecordType, null, 0, 0, 0);
                    }
                }
            }

            return null;
        }

        #endregion IMessageProvider Members
    }
}