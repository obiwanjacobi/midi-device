using System;
using System.IO;
using System.Reflection;
using CannedBytes.Midi.Device.Schema.Xml;

namespace CannedBytes.Midi.Device.Schema;

public static class DeviceSchemaImportResolver
{
    public static DeviceSchema LoadSchema(DeviceSchemaSet schemas, string name, string assembly)
    {
        using var stream = OpenSchema(name, assembly)
            ?? throw new DeviceSchemaException(
                $"Failed to open schema (import) {name} ({assembly}).");

        var parser = new MidiDeviceSchemaParser(schemas);
        var schema = parser.Parse(stream);
        return schema;
    }

    public static Stream OpenSchema(string name, string assemblyName)
    {
        Stream stream = null;

        if (string.IsNullOrEmpty(assemblyName) &&
            !Path.IsPathRooted(name))
        {
            name = Path.Combine(Environment.CurrentDirectory, name);
        }

        if (File.Exists(name))
        {
            stream = File.OpenRead(name);
        }
        else
        {
            Assembly assembly;
            if (string.IsNullOrEmpty(assemblyName))
            {
                assembly = Assembly.GetEntryAssembly();
            }
            else
            {
                // Assembly.Load() does not work (the same)...
                assembly = Assembly.LoadFrom(assemblyName + ".dll");
            }

            if (assembly != null)
            {
                stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + name);
            }
        }

        return stream;
    }
}
