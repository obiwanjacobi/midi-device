namespace CannedBytes.Midi.Device.Converters
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using CannedBytes.Midi.Device.Schema;

    [Export]
    public class ConverterManager
    {
#pragma warning disable 0649
        [ImportMany(typeof(ConverterFactory), AllowRecomposition = true)]
        private List<Lazy<ConverterFactory, IConverterFactoryInfo>> _factories;
#pragma warning restore 0649

        private Dictionary<string, Converter> _converterMap = new Dictionary<string, Converter>();
        private Dictionary<string, GroupConverter> _groupConverterMap = new Dictionary<string, GroupConverter>();

        public void InitializeFrom(CompositionContainer container)
        {
            Check.IfArgumentNull(container, "container");

            var converterMgr = container.GetExportedValueOrDefault<ConverterManager>();

            if (converterMgr != null)
            {
                // there is already a converter manager instance in the container.
                container.SatisfyImportsOnce(this);
            }
            else
            {
                // add this instance to the container
                var batch = new CompositionBatch();
                batch.AddExportedValue(this);
                container.Compose(batch);
            }
        }

        protected Converter LookupConverter(DataType dataType)
        {
            if (!_converterMap.ContainsKey(dataType.Name.FullName))
            {
                return null;
            }

            return _converterMap[dataType.Name.FullName];
        }

        protected GroupConverter LookupGroupConverter(RecordType recordType)
        {
            if (IsDynamic(recordType)) return null;

            if (!_groupConverterMap.ContainsKey(recordType.Name.FullName))
            {
                return null;
            }

            return _groupConverterMap[recordType.Name.FullName];
        }

        protected ConverterFactory LookupFactory(string schemaName)
        {
            if (_factories == null)
            {
                throw new InvalidOperationException("The converter manager has not been initialized. Call the InitializeFrom method.");
            }
            Debug.Assert(_factories.Count > 0, "No Converter Factories are registered.");

            foreach (var regInfo in _factories)
            {
                if (regInfo.Metadata.SchemaName == schemaName)
                {
                    return regInfo.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a pooled converter for the specified dataType.
        /// </summary>
        /// <param name="matchType">The <see cref="DataType"/> that is used to match to a factory and a converter instance.</param>
        /// <param name="constructType">The <see cref="DataType"/> that is used to construct the converter with.</param>
        /// <returns>Returns null if no converter was suited for the <paramref name="matchType"/>.</returns>
        protected virtual Converter Create(DataType matchType, DataType constructType)
        {
            Debug.Assert(LookupConverter(constructType) == null,
                "Calling ConverterManager.Create while converter in the cache for type: " + constructType.Name.FullName);

            var converter = CreateConverter(matchType, constructType);

            if (converter == null &&
                !constructType.Name.FullName.StartsWith(DeviceConstants.MidiTypesSchemaName))
            {
                // get default factory
                var factory = LookupFactory(DeviceConstants.MidiTypesSchemaName);

                Debug.Assert(factory != null, "Cannot find the default Converter Factory!");

                // use the default factory to create a GroupConverter
                converter = factory.Create(matchType, constructType);
            }

            return converter;
        }

        /// <summary>
        /// Creates a pooled converter for the specified matchType.
        /// </summary>
        /// <returns>Returns null if no converter was suited for the matchType.</returns>
        protected virtual GroupConverter Create(RecordType matchType, RecordType constructType)
        {
            Debug.Assert(LookupGroupConverter(constructType) == null,
                "Calling ConverterManager.Create while converter in the cache for type: " + constructType.Name.FullName);

            var converter = CreateGroupConverter(matchType, constructType);

            if (converter == null &&
                !constructType.Name.FullName.StartsWith(DeviceConstants.MidiTypesSchemaName))
            {
                // get default factory
                var factory = LookupFactory(DeviceConstants.MidiTypesSchemaName);

                Debug.Assert(factory != null, "Cannot find the default Converter Factory!");

                // use the default factory to create a GroupConverter
                converter = factory.Create(matchType, constructType);
            }

            if (converter != null)
            {
                CreateFieldConverterPairs(constructType, converter);
            }

            return converter;
        }

        protected Converter CreateConverter(DataType matchType, DataType constructType)
        {
            Converter converter = null;
            ConverterFactory factory = LookupFactory(matchType.Schema.SchemaName);

            if (factory != null)
            {
                converter = factory.Create(matchType, constructType);
            }

            if (converter == null && matchType.BaseType != null)
            {
                converter = Create(matchType.BaseType, constructType);
            }

            if (converter == null && matchType.IsExtension)
            {
                IConverterExtension extension = null;

                for (int i = 0; i < matchType.BaseTypes.Count; i++)
                {
                    var baseType = matchType.BaseTypes[i];
                    var innerConverter = Create(baseType, constructType);

                    if (innerConverter != null)
                    {
                        var innerExtension = innerConverter as IConverterExtension;

                        if (innerExtension != null)
                        {
                            if (extension != null)
                            {
                                extension.InnerConverter = innerExtension;
                            }
                            else
                            {
                                converter = innerConverter;
                            }

                            extension = innerExtension;
                        }
                        else
                        {
                            throw new MidiDeviceDataException(String.Format(
                                "Converter '{0}' does not support extensions but is used with the DataType '{1}' that is an extension.",
                                    innerConverter.GetType().FullName, baseType.Name));
                        }
                    }
                    else
                    {
                        throw new MidiDeviceDataException(String.Format(
                            "No Converter was found for the DataType '{0}'.", baseType.Name));
                    }
                }
            }

            return converter;
        }

        protected GroupConverter CreateGroupConverter(RecordType matchType, RecordType constructType)
        {
            GroupConverter converter = null;
            ConverterFactory factory = LookupFactory(matchType.Schema.SchemaName);

            if (factory != null)
            {
                converter = factory.Create(matchType, constructType);
            }

            if (converter == null && matchType.BaseType != null)
            {
                converter = CreateGroupConverter(matchType.BaseType, constructType);
            }

            return converter;
        }

        protected void CreateFieldConverterPairs(RecordType constructType, GroupConverter groupConverter)
        {
            // start at the base type(s) and add those fields first.
            if (constructType.BaseType != null && !constructType.IsDynamic)
            {
                CreateFieldConverterPairs(constructType.BaseType, groupConverter);
            }

            // We do not use FlattenedFields here.
            // We want to preserve the hierarchy including GroupConverters.
            foreach (Field field in constructType.Fields)
            {
                if (field.DataType != null)
                {
                    IConverter converter = GetConverter(field.DataType);

                    Debug.Assert(converter != null, "Could not find a suitable Converter for " + field.DataType.Name.FullName);

                    groupConverter.FieldConverterMap.Add(field, converter);
                }
                else if (field.RecordType != null)
                {
                    GroupConverter subGroupConverter = GetConverter(field.RecordType);

                    groupConverter.FieldConverterMap.Add(field, subGroupConverter);
                }
                else
                {
                    Debug.WriteLine(String.Format(
                        "WARNING: Field '{0}' did not have a DataType or RecordType set!", field.Name.FullName), "Converter Manager");
                }
            }
        }

        protected bool IsDynamic(RecordType constructType)
        {
            if (!constructType.IsDynamic)
            {
                foreach (var field in constructType.Fields)
                {
                    if (field.RecordType != null)
                    {
                        if (IsDynamic(field.RecordType))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a converter for the specified <paramref name="dataType"/>.
        /// </summary>
        /// <returns>Returns null if no converter suited for type.</returns>
        public Converter GetConverter(DataType dataType)
        {
            return GetConverter(dataType, dataType);
        }

        public Converter GetConverter(DataType matchType, DataType constructType)
        {
            Converter converter = LookupConverter(constructType);

            if (converter == null)
            {
                converter = Create(matchType, constructType);

                if (converter != null)
                {
                    _converterMap.Add(constructType.Name.FullName, converter);
                }
            }

            return converter;
        }

        /// <summary>
        /// Returns a converter container for the specified <paramref name="matchType"/>.
        /// </summary>
        /// <returns>Returns null if no converter suited for type.</returns>
        public GroupConverter GetConverter(RecordType recordType)
        {
            return GetConverter(recordType, recordType);
        }

        public GroupConverter GetConverter(RecordType matchType, RecordType constructType)
        {
            GroupConverter converter = LookupGroupConverter(constructType);

            if (converter == null)
            {
                converter = Create(matchType, constructType);

                if (converter != null && !IsDynamic(constructType))
                {
                    _groupConverterMap.Add(constructType.Name.FullName, converter);
                }
            }

            return converter;
        }

        public IConverter GetConverter(Field field)
        {
            if (field.DataType != null)
            {
                return GetConverter(field.DataType);
            }

            if (field.RecordType != null)
            {
                return GetConverter(field.RecordType);
            }

            return null;
        }
    }
}