using System;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device;

partial class LogicalFieldNode
{
    public sealed class LogicalValue
    {
        public object Data { get; set; }
        public int BitLength { get; set; }
        public int ByteLength { get; set; }

        public bool IsSignedValue { get; set; }
        // TODO: make custom enum and merge these props into one
        public TypeCode TypeCode { get; set; }
        public VarUInt64.VarTypeCode VarTypeCode { get; set; }
    }
}
