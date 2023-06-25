using System;
using System.Collections.Generic;
using System.Reflection;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace TestApp.MainView;

internal class MainViewModel : ViewModelBase
{
    private readonly static Assembly DeviceAssembly =
        Assembly.GetAssembly(typeof(DataConverter))!;

    public MainViewModel()
    {
        Services = ConfigureServices();
    }

    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDeviceSchemaProvider, DeviceSchemaProvider>();

        services.AddSingletonAll<DataConverter>(DeviceAssembly);
        services.AddSingletonAll<StreamConverter>(DeviceAssembly);
        services.AddSingletonAll<IConverterFactory>(DeviceAssembly);
        services.AddSingleton<ConverterManager>();

        services.AddSingleton<SchemaNodeMapFactory>();

        return services.BuildServiceProvider();
    }
}
