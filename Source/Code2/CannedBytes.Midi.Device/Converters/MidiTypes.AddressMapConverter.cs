﻿using System;
using System.Collections.Generic;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

internal sealed class AddressMapConverter : StreamConverter, INavigationEvents
{
    private const string ChildrenStateName = "AddressMapConverterChildren";

    public AddressMapConverter(RecordType constructType)
        : base(constructType)
    {
        IsAddressMap = true;
    }

    /// <summary>
    /// Override to provide dynamic address map content.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override IEnumerable<SchemaNode> GetChildNodeIterator(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        var children = context.StateMap.Get<IEnumerable<SchemaNode>>(ChildrenStateName);

        return children ?? base.GetChildNodeIterator(context);
    }

    public override void OnBeforeRecord(DeviceDataContext context)
    {
        Assert.IfArgumentNull(context, nameof(context));

        GetAddressAndSize(context, out SevenBitUInt32 address, out SevenBitUInt32 size);

        if (size == 0)
        {
            size = CalculateSize(context);
        }

        var mgr = new AddressMapManager(context.RootNode);

        var children = mgr.CreateSchemaNodes(address, size);

        context.StateMap.Set(ChildrenStateName, children);
    }

    private static void GetAddressAndSize(DeviceDataContext context, out SevenBitUInt32 address, out SevenBitUInt32 size)
    {
        var addressProperty = context.DeviceProperties.Find(DeviceProperty.AddressPropertyName);
        var sizeProperty = context.DeviceProperties.Find(DeviceProperty.SizePropertyName);

        if (addressProperty is null)
        {
            if (context.ConversionDirection == ConversionDirection.ToPhysical)
            {
                throw new DeviceDataException(
                    "No address was specified in the device properties to use for calculating the address map.");
            }

            throw new DeviceDataException(
                "The AddressMapConverter could not find the 'address' device property. " + Environment.NewLine +
                "Did you tag the field that contains the address with: property='address'?" + Environment.NewLine +
                "The field with the address property MUST occur before the address map starts.");
        }

        address = SevenBitUInt32.FromSevenBitValue(addressProperty.GetValue<uint>());

        if (sizeProperty is not null)
        {
            size = SevenBitUInt32.FromSevenBitValue(sizeProperty.GetValue<uint>());
        }
        else
        {
            size = SevenBitUInt32.FromInt32(0);
        }

        if (size == 0 &&
            context.ConversionDirection == ConversionDirection.ToPhysical)
        {
            throw new DeviceDataException(
                "No size was specified in the device properties to use for calculating the address map.");
        }
    }

    private SevenBitUInt32 CalculateSize(DeviceDataContext context)
    {
        throw new NotImplementedException();
    }
}
