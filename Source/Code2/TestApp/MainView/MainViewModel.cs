using System;
using System.Reflection;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;
using Microsoft.Devices.Midi2;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Services;

namespace TestApp.MainView;

internal class MainViewModel : ViewModel
{
    private readonly static Assembly DeviceAssembly =
        Assembly.GetAssembly(typeof(DataConverter))!;

    public MainViewModel()
    {
        Services = ConfigureServices();

        // MIDI2 experiment
        //Console.WriteLine($"MIDI2 SDK Version: {MidiServices.SdkVersion}");
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDeviceSchemaProvider, DeviceSchemaProvider>();
        services.AddSingletonAll<DataConverter>(DeviceAssembly);
        services.AddSingletonAll<StreamConverter>(DeviceAssembly);
        services.AddSingletonAll<IConverterFactory>(DeviceAssembly);
        services.AddSingleton<ConverterManager>();
        services.AddSingleton<SchemaNodeMapFactory>();

        services.AddSingleton<MidiService>();

        return services.BuildServiceProvider();
    }
}
