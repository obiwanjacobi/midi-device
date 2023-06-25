using System;
using System.IO;
using System.Reflection;

namespace CannedBytes.Midi.Device.Schema;

internal static class SchemaLocator
{
    public static Stream OpenSchemaStream(SchemaName schemaName)
    {
        var stream = schemaName.HasAssemblyName
            ? OpenAssemblyResource(schemaName.AssemblyName!, schemaName.FileName!)
            : OpenSchemaFileStream(schemaName.FileName!);

        return stream;
    }

    public static Stream? OpenAssemblyResource(string assemblyName, string fileName)
    {
        // Assembly.Load() does not work (the same)...
        var assembly = Assembly.LoadFrom(assemblyName + ".dll");

        Stream? stream = null;
        if (assembly != null)
        {
            stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{fileName}");
        }

        return stream;
    }

    public static Stream? OpenSchemaFileStream(string fileName)
    {
        if (!Path.IsPathRooted(fileName))
        {
            fileName = Path.Combine(Environment.CurrentDirectory, fileName);
        }

        if (File.Exists(fileName))
        {
            var stream = File.OpenRead(fileName);
            return stream;
        }

        return null;
    }
}
