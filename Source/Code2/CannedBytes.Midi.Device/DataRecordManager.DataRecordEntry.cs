using System;
using System.Text;

namespace CannedBytes.Midi.Device;

partial class DataRecordManager
{
    public class DataRecordEntry
    {
        public bool CarryCleared { get; set; }
        public object Data { get; set; }
        public SchemaNode Node { get; set; }
        public long PhysicalStreamPosition { get; set; }
        public long ParentStreamPosition { get; set; }
        public Exception Error { get; set; }
        public string Message { get; private set; }

        public DataRecordEntry Clone()
        {
            DataRecordEntry clone = new()
            {
                CarryCleared = CarryCleared,
                Data = Data,
                Error = Error,
                Node = Node,
                Message = Message,
                ParentStreamPosition = ParentStreamPosition,
                PhysicalStreamPosition = PhysicalStreamPosition
            };

            return clone;
        }

        public bool IsEmpty
        {
            get
            {
                return Data == null && Node == null && Error == null &&
                    String.IsNullOrEmpty(Message) &&
                    PhysicalStreamPosition == 0 &&
                    ParentStreamPosition == 0;
            }
        }

        public void AddMessage(string message)
        {
            if (String.IsNullOrEmpty(Message))
            {
                Message = message;
            }
            else
            {
                Message = Message + Environment.NewLine + message;
            }
        }

        public override string ToString()
        {
            var text = new StringBuilder();

            if (Node != null)
            {
                text.Append(Node.ToString());
                text.Append(" = ");
                text.Append(Data);
            }

            if (!String.IsNullOrEmpty(Message))
            {
                text.Append(" '");
                text.Append(Message);
                text.Append('\'');
            }

            if (Error != null)
            {
                text.Append(Error.ToString());
            }

            return text.ToString();
        }
    }
}
