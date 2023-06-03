namespace CannedBytes.Midi.Device.Schema
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The DeviceSchemaCollection class manages a collection of <see cref="DeviceSchema"/>
    /// instances.
    /// </summary>
    public class DeviceSchemaCollection : KeyedCollection<string, DeviceSchema>
    {
        /// <summary>
        /// Adds all <see cref="DeviceSchema"/> instance in the <paramref name="schemas"/>
        /// collection to this collection instance.
        /// </summary>
        /// <param name="schemas">A collection of <see cref="DeviceSchema"/>s. Must not be null.</param>
        public void AddRange(DeviceSchemaCollection schemas)
        {
            Check.IfArgumentNull(schemas, "schemas");

            foreach (DeviceSchema schema in schemas)
            {
                Add(schema);
            }
        }

        /// <summary>
        /// Tries to find the <see cref="DeviceSchema"/> instance with the
        /// specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of a DeviceSchema.</param>
        /// <returns>Returns null if the DeviceSchema could not be found.</returns>
        public DeviceSchema Find(string name)
        {
            if (base.Contains(name))
            {
                return base[name];
            }

            return null;
        }

        /// <summary>
        /// Called by the collection base class to determine the key for
        /// a <see cref="RecordType"/> item.
        /// </summary>
        /// <param name="item">The item from the collection.</param>
        /// <returns>Returns a unique key. Can return an empty string. Never returns null.</returns>
        /// <remarks>Override this method if you want the collection to use another property
        /// than <see cref="P:DeviceSchema.Name"/> as key.</remarks>
        protected override string GetKeyForItem(DeviceSchema item)
        {
            if (item != null)
            {
                return item.SchemaName;
            }

            return String.Empty;
        }
    }
}