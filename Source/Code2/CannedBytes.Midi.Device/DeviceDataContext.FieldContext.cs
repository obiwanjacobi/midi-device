using CannedBytes.Midi.Device.Schema;
using System;

namespace CannedBytes.Midi.Device
{
    partial class DeviceDataContext
    {
        /// <summary>
        /// Maintains field-specific context information.
        /// </summary>
        public sealed class FieldContext
        {
            private readonly DeviceDataContext _contex;

            internal FieldContext(DeviceDataContext context)
            {
                _contex = context;
            }

            public SchemaNode CurrentNode { get; set; }

            public Field CurrentField
            {
                get
                {
                    return CurrentNode == null ? null :
                        CurrentNode.FieldConverterPair.Field;
                }
            }

            public FieldValue<T> GetDataFieldValue<T>()
                where T : IComparable
            {
                var fieldValue = new FieldValue<T>(_contex);
                return fieldValue;
            }
        }
    }
}
