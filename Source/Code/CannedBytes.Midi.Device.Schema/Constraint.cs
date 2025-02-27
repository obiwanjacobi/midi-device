using System;

namespace CannedBytes.Midi.Device.Schema
{
    /// <summary>
    /// The Constraint class provides an abstract base class for concrete constraint
    /// implementations.
    /// </summary>
    public abstract class Constraint
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Constraint()
        { }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="validationType">A value indicating how to interpret the validation results.</param>
        protected Constraint(string name,
            ConstraintValidationType validationType)
        {
            Name = name;
            ValidationType = validationType;
        }

        /// <summary>
        /// Gets the value indicating how to interpret the validation results.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be empty.</value>
        public ConstraintValidationType ValidationType { get; internal protected set; }

        /// <summary>
        /// Gets the value indicating what standard type of constraint this is.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be empty.</value>
        public ConstraintType ConstraintType { get; internal protected set; }

        private string name;

        /// <summary>
        /// Gets the name of the Constraint.
        /// </summary>
        /// <value>Derived classes can set this property. Must not be null or empty.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            internal protected set
            {
                Check.IfArgumentNullOrEmpty(value, "Name");
                this.name = value;
            }
        }

        /// <summary>
        /// Gets the value used in validation.
        /// </summary>
        /// <remarks>Derived classes must implement this method.</remarks>
        public abstract T GetValue<T>();

        /// <summary>
        /// Validates the <paramref name="data"/> byte against the value.
        /// </summary>
        /// <param name="data">A 'unit' of midi data.</param>
        /// <returns>Returns true if the <paramref name="data"/> passes validation
        /// otherwise false is returned.</returns>
        public abstract bool Validate<T>(T data)
            where T : IComparable;
    }
}