# Midi Device

[ ] Carry will leave the previous low byte when reading in a high byte. This is correct for little endian but not for big endian. For big endian the carry should be cleared before a hibyte is read.

## Stream Stack

A typical stack looks like this:

- Physical Stream: the raw sysex bytes as received (always the root).
- Unpack Stream: Steam(Converters) that unpack 8 (or more) bits from the 7 bit sysex data.
- Endian Steam: Stream(Converter) that swaps bytes based on a specified width. We may deprecate this.
- Functional Stream: Stream(Converters) like SysEx and CheckSum etc.

> Note that currently the order in the stack is determined by the inheritance hierarchy of the RecordType in question.

## Byte Ordering (Endian-ness)

By default the physical stream presents bytes in the order they appear in the SysEx message.
The notion of endian-ness is relative to these bytes and the interpretation of the specific Midi Device. 

When a device presents its multi-bit/byte values in Little Endian, the Device Schema should reflect that and use all little endian data converters, except (typically) for strings.
When a device presents its data values in Big Endian, the Device Schema should use data converters that read in big endian format.

A Device Schema could have an attribute that defines the endian-ness globally for the entire model. The DeviceDataContext could have a central BitOrder field that Stream/Data-Converters can use to perform their read and write operations or that Stream/Data-Converters can manipulate (stack function) to temporarily change the endian-ness of the data operations.
