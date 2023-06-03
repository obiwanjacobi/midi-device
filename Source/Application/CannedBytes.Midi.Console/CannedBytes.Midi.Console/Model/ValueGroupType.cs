namespace CannedBytes.Midi.Console.Model
{
    /// <summary>
    /// Indicates how one or more values interact together.
    /// </summary>
    /// <remarks>Types can be extended by external components.
    /// That is why simple string are chosen.</remarks>
    public class ValueGroupType
    {
        protected ValueGroupType()
        { }

        /// <summary>
        /// Custom types should specify a standard fallback type.
        /// </summary>
        /// <param name="name">The name of the value group type.</param>
        /// <param name="fallbackName">A name of an existing group type that can be
        /// used if no controls are available for group type indicated by <paramref name="name"/>.</param>
        public ValueGroupType(string name, string fallbackName = null)
        {
            Name = name;
            FallbackName = fallbackName;
        }

        /// <summary>Each value is singular.</summary>
        public static readonly ValueGroupType Value = new ValueGroupType("Value");
        /// <summary>Two (or more) values compose a range (min/max).</summary>
        public static readonly ValueGroupType Range = new ValueGroupType("Range");

        public string Name { get; protected set; }

        public string FallbackName { get; protected set; }

        public override string ToString()
        {
            return Name + "[" + FallbackName + "]";
        }
    }
}