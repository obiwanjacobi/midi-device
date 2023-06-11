﻿using System;
using System.Reflection;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using CannedBytes.Midi.Device.Schema.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace CannedBytes.Midi.Device.IntegrationTests
{
    internal static class ServiceHelper
    {
        private readonly static Assembly DeviceAssembly = 
            Assembly.GetAssembly(typeof(DataConverter));
        private readonly static Assembly SchemaAssembly =
            Assembly.GetAssembly(typeof(DeviceSchema));

        public static IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDeviceSchemaProvider, MidiDeviceSchemaProvider>();

            services.AddSingleton<ConverterManager.AttributedConverterFactory>();
            services.AddSingletonAll<DataConverter>(DeviceAssembly);
            services.AddSingletonAll<StreamConverter>(DeviceAssembly);
            services.AddSingletonAll<IConverterFactory>(DeviceAssembly);
            services.AddSingleton<ConverterManager>();
            services.AddSingleton<SchemaNodeMapFactory>();

            return services.BuildServiceProvider();
        }
    }
}
