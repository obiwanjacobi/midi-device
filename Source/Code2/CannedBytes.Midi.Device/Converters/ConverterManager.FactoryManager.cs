using System;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private sealed class FactoryManager
    {
        private readonly List<IConverterFactory> _factories;
        private readonly AttributedConverterFactory _attributedFactory;

        public FactoryManager(AttributedConverterFactory attributedFactory,
            IEnumerable<IConverterFactory> factories)
        {
            Check.IfArgumentNull(attributedFactory, "attributedFactory");
            Check.IfArgumentNull(factories, "factories");

            _attributedFactory = attributedFactory;
            _factories = factories.ToList();

            DefaultFactory = Lookup(MidiTypes.MidiTypesSchemaName);

            if (DefaultFactory == null)
            {
                throw new DeviceException(
                    $"The default converter factory implementation was not found for schema: {MidiTypes.MidiTypesSchemaName}");
            }
        }

        public IConverterFactory DefaultFactory { get; }

        public IConverterFactory Lookup(string schemaName)
        {
            IEnumerable<IConverterFactory> all = LookupAll(schemaName);

            return all.FirstOrDefault();
        }

        public IEnumerable<IConverterFactory> LookupAll(string schemaName)
        {
            ThrowIfNotInitialized();
            List<IConverterFactory> factories = new();

            foreach (var factory in _factories)
            {
                if (factory.SchemaName == schemaName)
                {
                    factories.Add(factory);
                }
            }

            if (_attributedFactory.SchemaNames.Contains(schemaName))
            {
                factories.Add(_attributedFactory);
            }

            return factories;
        }

        private void ThrowIfNotInitialized()
        {
            if (_factories?.Any() != true)
            {
                throw new InvalidOperationException(
                    "The converter manager has not been initialized. No IConverterFactories were registered");
            }
        }
    }

    private sealed class _FactoryManager
    {
        private readonly IEnumerable<Lazy<IConverterFactory, IConverterFactoryInfo>> _factories;
        private readonly AttributedConverterFactory _attributedFactory;

        public _FactoryManager(AttributedConverterFactory attributedFactory,
            IEnumerable<Lazy<IConverterFactory, IConverterFactoryInfo>> factories)
        {
            Check.IfArgumentNull(attributedFactory, "attributedFactory");
            Check.IfArgumentNull(factories, "factories");

            _attributedFactory = attributedFactory;
            _factories = factories;

            DefaultFactory = Lookup(MidiTypes.MidiTypesSchemaName);

            if (DefaultFactory == null)
            {
                throw new DeviceException(
                    $"The default converter factory implementation was not found for schema: {MidiTypes.MidiTypesSchemaName}");
            }
        }

        public IConverterFactory DefaultFactory { get; }

        public IConverterFactory Lookup(string schemaName)
        {
            IEnumerable<IConverterFactory> all = LookupAll(schemaName);

            return all.FirstOrDefault();
        }

        public IEnumerable<IConverterFactory> LookupAll(string schemaName)
        {
            ThrowIfNotInitialized();
            List<IConverterFactory> factories = new();

            foreach (Lazy<IConverterFactory, IConverterFactoryInfo> regInfo in _factories)
            {
                if (regInfo.Metadata.SchemaName == schemaName)
                {
                    factories.Add(regInfo.Value);
                }
            }

            if (_attributedFactory.SchemaNames.Contains(schemaName))
            {
                factories.Add(_attributedFactory);
            }

            return factories;
        }

        private void ThrowIfNotInitialized()
        {
            if (_factories?.Any() != true)
            {
                throw new InvalidOperationException(
                    "The converter manager has not been initialized. No IConverterFactories were registered");
            }
        }
    }
}
