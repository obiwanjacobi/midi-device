# Midi Device

[ ] `BitConverter`: implement bitOrder attribute handling (or create a different converter).

[ ] `SchemaNodeMap`: There is an inconsistency in the node the `NextClone` property points to. At the first (real) repeated node the `NextClone` points to the next node at the same parent-level. However clones of that node point the `NextClone` to the next clone node at the sibling-level. Perhaps create individual properties to hook up clones at the same parent-level and one at the same sibling-level. `NextClone` would point at sibling-level clones while `` would point at parent-level clones.

[ ] Allow some parts (RecordType) of an address map to be readonly, writeonly or read/write (See Roland FC-300 manual).


## Done

[x] SchemaNodeMap: InstanceKeyPath gives wrong index value for repeated records with sub-records. Example: D110-PatchParametersPart1. Looks like the repeated index is used for the sub-level: sub-record 0|63|0|0 then field 63|63|0|0.


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
