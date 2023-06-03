namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The FieldConverterMap class manages a collection of <see cref="FieldConverterPair"/> instances
    /// mapped to the full name of the <see cref="Field"/>.
    /// </summary>
    public class FieldConverterMap : KeyedCollection<string, FieldConverterPair>
    {
        /// <summary>
        /// Add a map item for the specified <paramref name="field"/> and <paramref name="converter"/>.
        /// </summary>
        /// <param name="field">Must not be null.</param>
        /// <param name="converter">Must not be null.</param>
        /// <returns>Returns the <see cref="FieldConverterPair"/> stored in the map.</returns>
        public FieldConverterPair Add(Field field, IConverter converter)
        {
            ThrowIfLocked();
            FieldConverterPair pair = new FieldConverterPair(field, converter);

            base.Add(pair);

            return pair;
        }

        /// <summary>
        /// Adds the contents of the specified <paramref name="map"/> to this instance.
        /// </summary>
        /// <param name="map">Must not be null.</param>
        public void AddRange(FieldConverterMap map)
        {
            Check.IfArgumentNull(map, "map");

            foreach (FieldConverterPair pair in map)
            {
                Add(pair);
            }
        }

        internal bool Locked { get; set; }

        private void ThrowIfLocked()
        {
            if (Locked)
            {
                throw new InvalidOperationException("The map is locked - being enumerated.");
            }
        }

        protected override void ClearItems()
        {
            ThrowIfLocked();
            base.ClearItems();
        }

        protected override void InsertItem(int index, FieldConverterPair item)
        {
            ThrowIfLocked();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfLocked();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, FieldConverterPair item)
        {
            ThrowIfLocked();
            base.SetItem(index, item);
        }

        /// <summary>
        /// Retrieves the <see cref="FieldConverterPair"/> that matches the specified <paramref name="field"/>.
        /// </summary>
        /// <param name="field">Must not be null.</param>
        /// <returns>Returns null if no matching pair was found.</returns>
        public FieldConverterPair Find(Field field)
        {
            Check.IfArgumentNull(field, "field");

            return Find(field.Name.FullName);
        }

        /// <summary>
        /// Retrieves the <see cref="FieldConverterPair"/> that matches the specified <paramref name="fullFieldName"/>.
        /// </summary>
        /// <param name="fullFieldName">Must not be null or empty.</param>
        /// <returns>Returns null if no matching pair was found.</returns>
        public FieldConverterPair Find(string fullFieldName)
        {
            Check.IfArgumentNullOrEmpty(fullFieldName, "fullFieldName");

            if (Contains(fullFieldName))
            {
                return this[fullFieldName];
            }

            return null;
        }

        /// <summary>
        /// Finds all duplicates that start with the <paramref name="fullFieldName"/>.
        /// </summary>
        /// <param name="fullFieldName">Must not be null or empty.</param>
        /// <returns>Never returns null.</returns>
        public IEnumerable<FieldConverterPair> FindAll(string fullFieldName)
        {
            Check.IfArgumentNullOrEmpty(fullFieldName, "fullFieldName");

            var result = from pair in this.Dictionary
                         where pair.Key.StartsWith(fullFieldName)
                         select pair.Value;

            return result;
        }

        /// <summary>
        /// Finds a <see cref="FieldConverterPair"/> by <see cref="RecordType"/>.
        /// </summary>
        /// <param name="recordType">Must not be null.</param>
        /// <param name="recursive">True if nested records should also be searched.</param>
        /// <returns>Returns null if no match is found.</returns>
        public FieldConverterPair FindByType(RecordType recordType, bool recursive)
        {
            return FindByType(this, recordType, recursive);
        }

        protected static FieldConverterPair FindByType(FieldConverterMap fieldConverterMap, RecordType recordType, bool recursive)
        {
            FieldConverterPair pairFound = null;

            foreach (var pair in fieldConverterMap)
            {
                var groupConverter = pair.Converter as GroupConverter;

                if (groupConverter != null)
                {
                    if (pair.Field.RecordType.IsType(recordType))
                    {
                        pairFound = pair;
                    }
                    else if (recursive)
                    {
                        pairFound = FindByType(groupConverter.FieldConverterMap, recordType, true);
                    }
                }

                if (pairFound != null)
                {
                    break;
                }
            }

            return pairFound;
        }

        /// <summary>
        /// Retrieves the identifying string from the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Must not be null.</param>
        /// <returns>Returns the full name of the field and optionally a postfix to make it unique.</returns>
        protected override string GetKeyForItem(FieldConverterPair item)
        {
            if (item != null && item.Field != null)
            {
                string key = item.Field.Name.FullName;

                if (base.Contains(key))
                {
                    // put a limit to it
                    for (int index = 1; index < 65535; index++)
                    {
                        key = String.Format("{0}_{1}", item.Field.Name.FullName, index);

                        if (!base.Contains(key))
                        {
                            return key;
                        }
                    }
                }
                else
                {
                    return key;
                }
            }

            return null;
        }
    }
}