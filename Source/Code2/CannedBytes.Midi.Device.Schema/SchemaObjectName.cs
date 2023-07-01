using System;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// Manages the schema (namespace) and object name.
/// </summary>
public sealed class SchemaObjectName
{
    private const char SchemaNameSeparator = ':';

    public SchemaObjectName(string fullName)
    {
        int index = fullName.LastIndexOf(SchemaNameSeparator);
        if (index < 0)
        {
            throw new ArgumentException(
                $"Cannot parse fullName: {fullName}");
        }

        Name = fullName[(index + 1)..];
        SchemaName = fullName[..index];
        FullName = fullName;
    }

    public SchemaObjectName(string schemaName, string objectName)
    {
        SchemaName = schemaName;
        Name = objectName;
        FullName = schemaName + SchemaNameSeparator + objectName;
    }

    public string Name { get; }

    public string SchemaName { get; }

    public string FullName { get; }

    public override string ToString()
    {
        return FullName;
    }
}