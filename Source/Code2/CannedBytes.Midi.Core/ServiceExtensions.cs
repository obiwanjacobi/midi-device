using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CannedBytes.Midi.Core;

public static class ServiceExtensions
{
    public static ServiceCollection AddSingletonAll<InterfaceT>(this ServiceCollection services, Assembly assemblyToScan)
    {
        var typeToTest = typeof(InterfaceT);
        var types = ScanAssemblyFor(assemblyToScan, typeToTest);

        foreach (var type in types)
        {
            services.AddSingleton(typeToTest, type);
        }

        return services;
    }

    private static IEnumerable<Type> ScanAssemblyFor(Assembly assemblyToScan, Type typeToTest)
    {
        if (typeToTest.IsAbstract)
        {
            return assemblyToScan.GetExportedTypes()
                .Where(t => t.IsAssignableTo(typeToTest)
                    && !t.IsAbstract
                    && t != typeToTest);
        }
        else
        {
            return assemblyToScan.GetExportedTypes()
                .Where(t => t.IsAssignableTo(typeToTest)
                    && !t.IsAbstract);
        }
    }
}
