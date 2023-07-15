# Midi Device

The aim of this project is to provide a way to get and set individual settings of any Midi Device that supports Midi SysEx and or Midi2.

## Midi Console Application

Several parts to this app:

- Midi Device Schema Editor
- Midi Device Test Bench
- Midi Device Explorer
- Dashboard Editor

### Midi Device Schema Editor

A way to create Midi Device Schemas for any Midi Device.
Allows to incrementally build up a Midi Device Schema based on (SysEx) data retrieved from the device.
Supports reverse engineering the Midi Device Schema or just entering it from a detailed manual (like the old Roland manuals).

The Midi Device Schema may also include short Midi commands and Midi Message control flow.

### Midi Device Test Bench

Here a Midi Device Schema can be fully tested against the intended device.
Check to see if all physical to logical (and vs) value conversions are done correctly.

(Could get integrated into the Midi Device Schema Editor)

### Midi Device Explorer

Based on a Midi Device Schema, the Midi Device is read fully and all its settings are displayed in a orderly fashion.
One can search and order these settings.

Multiple devices can be explored at the same time.

This is input for the Dashboard Editor

### Dashboard Editor

A 'graphical' design tool that allows creation of control surfaces for multiple Midi Devices.

A Dashboard design can be saved and run with a player that will allow controling the surface but not editing the design.
The player is targeted to be used at (live) performances.
