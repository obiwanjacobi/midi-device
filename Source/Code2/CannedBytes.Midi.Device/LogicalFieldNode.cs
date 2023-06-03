using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;
using System;
using System.Collections.Specialized;

namespace CannedBytes.Midi.Device
{
    public partial class LogicalFieldNode : ILogicalFieldInfo
    {
        public LogicalFieldNode Parent { get; set; }
        public LogicalFieldNode Next { get; set; }
        public LogicalFieldNode Previous { get; set; }

        public SchemaNode SchemaNode { get; set; }

        public Field Field { get; set; }
        public InstancePathKey Key { get; set; }

        public LogicalValue Value { get; set; }

        public bool CarryFlushed { get; set; }
        public long PhysicalStreamPosition { get; set; }
        public long ParentStreamPosition { get; set; }

        public Exception Error { get; set; }
        public string DiagnosticMessage { get; set; }

        ILogicalFieldInfo ILogicalFieldInfo.Parent
        {
            get { return Parent; }
        }
    }
}
