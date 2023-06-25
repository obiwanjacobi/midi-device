using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema;

/// <summary>
/// The IDeviceSchemaProvider interface provides access to Device Schema information
/// and is implemented by classes that provide concrete implementations for Device Schema
/// technologies.
/// </summary>
/// <remarks>Device Schema Providers can provide more than one type of
/// <see cref="DeviceSchema"/>. It is expected (but not mandatory) that loaded
/// Device Schemas are cached for reuse.</remarks>
public interface IDeviceSchemaProvider
{
    /// <summary>
    /// Retrieves a list of the name of all available schemas.
    /// </summary>
    IEnumerable<string> SchemaNames { get; }

    /// <summary>
    /// Loads the <see cref="DeviceSchema"/> with the specified <paramref name="schemaName"/>.
    /// </summary>
    /// <param name="schemaName">The location of the schema. Must not be empty.</param>
    /// <returns>Returns null if the provider was unable to load the schema.</returns>
    DeviceSchema Load(SchemaName schemaName);

    /// <summary>
    /// Retrieves the <see cref="DeviceSchema"/> with the specified <paramref name="schemaName"/>
    /// from the already loaded schemas or tries to Load it if not available yet.
    /// </summary>
    /// <param name="schemaName">The name of the schema. Must not be null or empty.</param>
    /// <returns>Returns null if the provider was unable to locate the schema.</returns>
    DeviceSchema Open(SchemaName schemaName);

    /// <summary>
    /// Retrieves the <see cref="RecordType"/> with the <paramref name="typeName"/>
    /// from the <see cref="DeviceSchema"/> identified by the <paramref name="schemaName"/>.
    /// </summary>
    /// <param name="schemaName">The name of the schema the RecordType is part of.</param>
    /// <param name="typeName">The name of the RecordType to fetch.</param>
    /// <returns>Returns null if either the <paramref name="schemaName"/> or
    /// <paramref name="typeName"/> were not found.</returns>
    RecordType FindRecordType(string schemaName, string typeName);

    /// <summary>
    /// Retrieves the <see cref="DataType"/> with the <paramref name="typeName"/>
    /// from the <see cref="DeviceSchema"/> identified by the <paramref name="schemaName"/>.
    /// </summary>
    /// <param name="schemaName">The name of the schema the DataType is part of.</param>
    /// <param name="typeName">The name of the DataType to fetch.</param>
    /// <returns>Returns null if either the <paramref name="schemaName"/> or
    /// <paramref name="typeName"/> were not found.</returns>
    DataType FindDataType(string schemaName, string typeName);
}