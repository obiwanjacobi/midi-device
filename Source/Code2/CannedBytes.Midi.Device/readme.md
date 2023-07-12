# Midi Device

[ ] `BitConverter`: implement bitOrder attribute handling (or create a different converter).

[ ] `SchemaNodeMap`: There is an inconsistency in the node the `NextClone` property points to. At the first (real) repeated node the `NextClone` points to the next node at the same parent-level. However clones of that node point the `NextClone` to the next clone node at the sibling-level. Perhaps create individual properties to hook up clones at the same parent-level and one at the same sibling-level. `NextClone` would point at sibling-level clones while `` would point at parent-level clones.

[ ] `DeviceSchema`: Allow some parts (RecordType) of an address map to be readonly, writeonly or read/write (See Roland FC-300 manual).

[ ] `DeviceSchema`: Overwrite inherited constraints (from the base type) of the same type with constraint defined at the current type.

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

## Logical Data Representation

The following scenario's:

1) Create a logical message from scratch. Using the address-map to pinpoint the instance (index).
1) Have all the logical information that was captured in the DeviceSchema available to the application during ToLogical/ToPhysical processes.

Logical DeviceSchema information consists of:

- Constraints: Enumerated (with name) and min/max and fixed value, length
- valueOffset: a shift of the logical value from its physical value.
- range: a physical value range.
- Field: Field attributes incl. RecordType/DataType attributes. DeviceProperty.
- address, size and repeats.
- DeviceSchema documentation: possibly in multiple languages.

DeviceSchema => SchemaNodeMap => LogicalFieldNode + LogicalFieldValue

It should be clear what fields require a (logical) value and which fields do not - because they're fixed or dummy fields.

## Application Perspective

For the applications point of view some facilities need to be in place to work efficiently with the Midi Device Schema mechanism.

The Device Schema has to be available and searchable. It should be easy to find a specific field.

When using an address map the application needs to be able to find the address (and size) for the n-th instance of a field or a (partial) set of fields. Imagine the logical fields displayed as editable controls on the screen and the user changing one of its values. That change (and only that change) should be passed onto the device using the address map.

Ideally this code should be encapsulated in a simple to use API:

```csharp
  var deviceSchema = /* Open a Device Schema that represents the midi device */;

  // create a device provider for a specific midi device (schema)
  var deviceProvider = DeviceProvider.Create(Services, deviceSchema);

  var recordType = deviceProvider.Schema.RootRecordTypes.Find("SendDataRecord");
  var binMap = deviceProvider.GetBinaryMap(recordType);

  // instance index key determines the exact location in the address map.
  var instanceIndexKey = new InstanceIndexKey(0, 1, 4, 0);
  var addressMapNodes = binMap.GetSchemaNodes(instanceIndexKey);
```

```csharp
  var midiDevice = MidiDevice.Create(Services, deviceSchema);
  var addressMapNodes = midiDevice.GetAddressMapNodes("SendDataRecords, new InstanceIndexKey(0, 1, 4, 0));
```