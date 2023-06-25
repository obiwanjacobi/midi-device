using System;

namespace CannedBytes.Midi.Device.Schema;

public readonly struct SchemaName
{
    private SchemaName(string? schemaNamespace, string? fileName, string? assemblyName)
    {
        SchemaNamespace = schemaNamespace;
        FileName = fileName;
        AssemblyName = assemblyName;
    }

    public bool IsSingleSchema => HasSchemaNamespace || HasFileName;
    public bool IsMultipleSchemas => HasAssemblyName && !HasSchemaNamespace && !HasFileName;

    public bool HasSchemaNamespace => !String.IsNullOrEmpty(SchemaNamespace);
    public string? SchemaNamespace { get; }

    public bool HasFileName => !String.IsNullOrEmpty(FileName);
    public string? FileName { get; }

    public bool HasAssemblyName => !String.IsNullOrEmpty(AssemblyName);
    public string? AssemblyName { get; }

    public static SchemaName FromSchemaNamespace(string schemaNamespace)
        => new SchemaName(schemaNamespace, null, null);

    public static SchemaName FromFileName(string fileName)
        => new SchemaName(null, fileName, null);

    public static SchemaName FromAssemblyResource(string assemblyName, string fileName)
        => new SchemaName(null, fileName, assemblyName);

    public override string ToString()
        => $"Assembly={AssemblyName}, FileName={FileName}, Namespace={SchemaNamespace}";
}
