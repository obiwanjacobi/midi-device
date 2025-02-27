﻿using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.Core;

namespace CannedBytes.Midi.Device.Converters;

partial class ConverterManager
{
    private sealed class FactoryManager
    {
        private readonly List<IConverterFactory> _factories;

        public FactoryManager(IEnumerable<IConverterFactory> factories)
        {
            Assert.IfArgumentNull(factories, nameof(factories));

            _factories = factories.ToList();

            DefaultFactory = Lookup(MidiTypes.MidiTypesSchemaName)
                ?? throw new DeviceException(
                    $"The default converter factory implementation was not found for schema: {MidiTypes.MidiTypesSchemaName}");
        }

        public IConverterFactory DefaultFactory { get; }

        public IConverterFactory? Lookup(string schemaName)
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
