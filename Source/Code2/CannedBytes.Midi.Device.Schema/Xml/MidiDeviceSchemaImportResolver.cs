using System;
using System.IO;
using System.Reflection;

namespace CannedBytes.Midi.Device.Schema.Xml;

public static class MidiDeviceSchemaImportResolver
{
    public static DeviceSchema LoadSchema(MidiDeviceSchemaSet schemas, string name, string assembly)
    {
        using var stream = MidiDeviceSchemaImportResolver.OpenSchema(name, assembly);
        if (stream == null)
        {
            throw new DeviceSchemaException(
                String.Format("Failed to open schema (import) {0} ({1}).", name, assembly));
        }

        var parser = new MidiDeviceSchemaParser(schemas);
        var schema = parser.Parse(stream);

        //if (schema != null)
        //{
        //    _schemas.Add(schema);
        //}

        return schema;
    }

    public static Stream OpenSchema(string name, string assemblyName)
    {
        Stream stream = null;

        if (String.IsNullOrEmpty(assemblyName) &&
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
            Assembly assembly = null;

            if (String.IsNullOrEmpty(assemblyName))
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
