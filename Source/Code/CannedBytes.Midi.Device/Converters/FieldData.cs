﻿using System;
using System.Linq;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters
{
    /// <summary>
    /// Use this class to access Field information during a <see cref="ToLogical"/> or a <see cref="ToPhysical"/> conversion.
    /// </summary>
    /// <typeparam name="T">The data type of the field.</typeparam>
    public class FieldData<T>
        where T : IComparable
    {
        public FieldData(MidiDeviceDataContext context)
            : this(context.CurrentFieldConverter.Field)
        {
            if (!String.IsNullOrEmpty(this.Field.DevicePropertyName))
            {
                var prop = context.DeviceProperties.Find(this.Field.DevicePropertyName);

                if (prop != null)
                {
                    FixedValue = prop.GetValue<T>();
                    Callback = false;
                }
            }

            if (context.ForceLogicCall)
            {
                Callback = true;
            }
        }

        protected FieldData(Field field)
        {
            this.Field = field;

            Callback = true;

            var minValue = this.Field.Constraints.Find(ConstraintType.MinInclusive);

            if (minValue != null)
            {
                MinValue = minValue.GetValue<T>();
            }

            var maxValue = this.Field.Constraints.Find(ConstraintType.MaxInclusive);

            if (maxValue != null)
            {
                MaxValue = maxValue.GetValue<T>();
            }

            var fixValue = this.Field.Constraints.Find(ConstraintType.FixedValue);

            if (fixValue != null)
            {
                FixedValue = fixValue.GetValue<T>();
                Callback = false;
            }

            var enums = this.Field.Constraints.FindAll(ConstraintType.Enumeration);

            if (enums != null && enums.Count() == 1)
            {
                FixedValue = enums.First().GetValue<T>();
                Callback = false;
            }

            if (this.Field.DataType.Name.Name == "midiNull")
            {
                IsNullType = true;
                Callback = false;
            }
        }

        protected Field Field { get; private set; }

        /// <summary>
        /// Tests if the provided <paramref name="data"/> is valid.
        /// </summary>
        /// <param name="data">The data for this field.</param>
        /// <returns>Returns true if the data is valid.</returns>
        public bool IsValid(T data)
        {
            var strData = data as string;

            if (strData != null)
            {
                return this.Field.Constraints.Validate(strData.Length);
            }

            return this.Field.Constraints.Validate<T>(data);
        }

        public void Validate(T data)
        {
            if (!IsValid(data))
            {
                throw new MidiDeviceDataException(
                    String.Format("Validation failed for {0} and value {1}.", Field.Name.FullName, data));
            }
        }

        /// <summary>
        /// Set to the value of the MinInclusive constraint (if available).
        /// </summary>
        public T MinValue { get; private set; }

        /// <summary>
        /// Set to the value of the MaxInclusive constraint (if available).
        /// </summary>
        public T MaxValue { get; private set; }

        /// <summary>
        /// Set to the value of the Fixed value or single Enumeration constraint (if available).
        /// </summary>
        public T FixedValue { get; private set; }

        /// <summary>
        /// Indicates if the call to the logic reader/writer should be made.
        /// </summary>
        public bool Callback { get; private set; }

        /// <summary>
        /// True when the data type is a null-type.
        /// </summary>
        public bool IsNullType { get; private set; }
    }
}