using System;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device;

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

        public SchemaNode? CurrentNode { get; set; }

        public Field CurrentField
        {
            get
            {
                if (CurrentNode is not null)
                    return CurrentNode.FieldConverterPair.Field;

                throw new InvalidOperationException(
                    "The CurrentNode property must be set before accessing the CurrentField property.");
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
