using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.ComponentModel.Composition;
using CannedBytes.Midi.Device.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace CannedBytes.Midi.Device;

/// <summary>
/// An instance for each open device to communicate with.
/// </summary>
public sealed class DeviceProvider
{
    // device and manufacturer information
    public string Manufacturer { get; set; }

    public uint ManufacturerId { get; set; }

    public string ModelName { get; set; }

    public uint ModelId { get; set; }

    // device id? (different per device. should need only one DeviceProvider per device type.
    // perhaps have a list of id's mapped to logical names?
    public byte DeviceId { get; set; }

    // MidiDeviceSchema for device (type)
    public DeviceSchema Schema { get; private set; }

    public IEnumerable<SchemaNodeMap> BinaryMaps { get; private set; }

    // BinaryConverterMap / BinaryConverterView
    // complete structure for each root message.
    public SchemaNodeMap GetBinaryConverterMapFor(string virtualRootFieldName)
    {
        Check.IfArgumentNullOrEmpty(virtualRootFieldName, "virtualRootFieldName");

        Field virtualField = Schema.VirtualRootFields.Find(virtualRootFieldName);

        return virtualField == null
            ? throw new ArgumentException(
                $"The specified virtual root field name was not found in the schema: {virtualRootFieldName}", "virtualRootFieldName")
            : GetBinaryConverterMapFor(virtualField);
    }

    public SchemaNodeMap GetBinaryConverterMapFor(Field virtualRootField)
    {
        SchemaNodeMap map = (from bcm in BinaryMaps
                   where bcm.RootNode.FieldConverterPair.Field == virtualRootField
                   select bcm).FirstOrDefault();

        return map;
    }

    public static DeviceProvider Create(CompositionContext compositionContext, string schemaLocation)
    {
        DeviceProvider deviceProvider = new();

        IDeviceSchemaProvider schemaProvider = compositionContext.GetInstance<IDeviceSchemaProvider>();
        deviceProvider.Schema = schemaProvider.Load(schemaLocation);

        // filter root fields on 'midiSysEx' records
        List<Field> remove = (from vrf in deviceProvider.Schema.VirtualRootFields
                      where !vrf.RecordType.IsType(MidiTypes.MidiTypesSchema_SysEx)
                      select vrf).ToList();

        foreach (Field nonSysExRoot in remove)
        {
            deviceProvider.Schema.VirtualRootFields.Remove(nonSysExRoot);
        }

        if (deviceProvider.Schema.VirtualRootFields.Count == 0)
        {
            throw new DeviceDataException(
                $"The schema '{deviceProvider.Schema.SchemaName}' loaded from '{schemaLocation}' Does not contain any root records that derive from midiSysEx.");
        }

        SchemaNodeMapFactory mapFactory = compositionContext.GetInstance<SchemaNodeMapFactory>();
        deviceProvider.BinaryMaps = mapFactory.CreateAll(deviceProvider.Schema);

        return deviceProvider;
    }

    public static DeviceProvider Create(IServiceProvider serviceProvider, string schemaLocation)
    {
        DeviceProvider deviceProvider = new();

        IDeviceSchemaProvider schemaProvider = serviceProvider.GetRequiredService<IDeviceSchemaProvider>();
        deviceProvider.Schema = schemaProvider.Load(schemaLocation);

        // filter root fields on 'midiSysEx' records
        List<Field> remove = (from vrf in deviceProvider.Schema.VirtualRootFields
                              where !vrf.RecordType.IsType(MidiTypes.MidiTypesSchema_SysEx)
                              select vrf).ToList();

        foreach (Field nonSysExRoot in remove)
        {
            deviceProvider.Schema.VirtualRootFields.Remove(nonSysExRoot);
        }

        if (deviceProvider.Schema.VirtualRootFields.Count == 0)
        {
            throw new DeviceDataException(
                $"The schema '{deviceProvider.Schema.SchemaName}' loaded from '{schemaLocation}' Does not contain any root records that derive from midiSysEx.");
        }

        SchemaNodeMapFactory mapFactory = serviceProvider.GetRequiredService<SchemaNodeMapFactory>();
        deviceProvider.BinaryMaps = mapFactory.CreateAll(deviceProvider.Schema);

        return deviceProvider;
    }
}
