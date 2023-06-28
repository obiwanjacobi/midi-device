# Midi Types

This document describes the Midi Data Types that come with the Midi Device Schema.

A Field is an identifier that is either a Record Type or a Data Type.
The Field name should correspond to the setting in the Midi Device in question.

A Record Type is groupings of one or more Fields.
As with Data Types, Record Types come in different 'types'. 
Each of these Record Types perform a function on the stream of bytes before these bytes are passed to the Data Types (Converters).

A Data Type represents how physical data from the System Exclusive message is transformed into a logical value.
Each different Data Type (name) represents a different conversion of the physical data into a logical value.

Record Types as well as Data Types support inheritance to either extend or restrict the base type.

## Record Types

[ ] Mcoded7 (Midi2 spec)

## Data Types

[ ] Allow to indicate multiple bytes to be dummies or reserved.
  midiNull could derive from midiComposite and specify a `length` attribute.


### Signed Data Types

The Roland R-8 uses these:

| Bits | Description | Range |
| -- | -- | -- |
| 0aaa-aaaa | absolute value | 0-99
| 0000-000a | sign value | 0=pos, 1=neg

| Bits | Description | Range |
| -- | -- | -- |
| 0000-aaaa | value bits 0-3 |
| 0000-bbbb | value bits 4-7 | 0-480
| 0000-000c | value bit 8 |
| 0000-000a | sign value | 0=pos, 1=neg

| Bits | Description | Range |
| -- | -- | -- |
| 0aaa-aaaa | signed value | -63 - +63

| Bits | Description | Range |
| -- | -- | -- |
| 0000-aaaa | signed value | -7 - +7


### String Data Types

Restrict strings to a predefined character set / range:

For ASCII: 32-127


Allow other encodings that just ASCII (Midi2):

UTF-16: "\u..."


Null-terminated strings:

| Bits | Description | Range |
| -- | -- | -- |
| 0aaa-aaaa | char value | 32-127
| .. | .. | ..
| 0aaa-aaaa | char value | 32-127
| 0000-0000 | terminator | 0

Fill-out the character positions not used with 0 or space?


### Miscellaneous Data Types

Some bits have a constant value:

| Bits | Description | Range |
| -- | -- | -- |
| 0010-aaaa | value | 0-15

Can be solved with dummy fields - that are not pushed to the app layer.


Byte value maps to a virtual logical range:

| Bits | Description | Range |
| -- | -- | -- |
| 0aaa-aaaa | tuner value | 0-127 (432.1Hz-437.6Hz)

This requires a linear translation.


A value range combined with a logical state:

| Bits | Description | Range |
| -- | -- | -- |
| 000a-aaaa | value | 0-16, OFF

The highest bit indicates the off-state.


Representing a 7-segment led display digit:

| Bits | Description | Range |
| -- | -- | -- |
| 0abc-defg | 7-seg | 0-127
| 0000-000h | dot | 0-1

```
   d          a  
  ---        ---
e |  | c   f |  | b
  ---  a     ---  g
f |  | b   e |  | c
  --- .      ---
   g   h      d
```

Logical values could map to an ascii character for each possible combination .'"?=-_|#[], 0-9, AbcdeFghHiIjJLptuUY (doesn't cover every combination!)
Or represent the value with a string "a-h" representing all active segments.


