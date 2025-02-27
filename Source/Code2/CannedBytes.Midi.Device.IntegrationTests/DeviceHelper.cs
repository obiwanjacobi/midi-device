﻿using System;
using System.IO;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.IntegrationTests;

internal static class DeviceHelper
{
    public static DeviceDataContext ToLogical(
        IServiceProvider serviceProvider,
        string schemaLocation,
        string binStreamPath,
        string virtualRootName,
        IMidiLogicalWriter writer)
    {
        var deviceProvider = DeviceProvider.Create(serviceProvider, SchemaName.FromFileName(schemaLocation));
        var binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

        var process = new DeviceToLogicalProcess();

        using var stream = File.OpenRead(binStreamPath);
        var dataCtx = process.Execute(binMap.RootNode, stream, writer);

        return dataCtx;
    }

    public static DeviceDataContext ToPhysical(
        IServiceProvider serviceProvider,
        string schemaLocation,
        string virtualRootName,
        IMidiLogicalReader reader)
    {
        var deviceProvider = DeviceProvider.Create(serviceProvider, SchemaName.FromFileName(schemaLocation));
        var binMap = deviceProvider.GetBinaryConverterMapFor(virtualRootName);

        var process = new DeviceToPhysicalProcess();

        var stream = new MemoryStream();
        var dataCtx = process.Execute(binMap.RootNode, stream, reader);

        return dataCtx;
    }
}
