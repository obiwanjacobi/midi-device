# Midi Device

## Byte Ordering (Endianness)

The `Stream` that is used to read and write the SysEx messages uses the same byte order as the platform it runs on.
That means a DeviceSchema should always explicitly declare in what byte-order the values are meant to be read/written.

Because the stream processing always requires to potentially change the byte order, there is no need for an explicit
Little/BigEndianConverter/Stream. There is always a root StreamConverter present that adjust the byte order as needed.
