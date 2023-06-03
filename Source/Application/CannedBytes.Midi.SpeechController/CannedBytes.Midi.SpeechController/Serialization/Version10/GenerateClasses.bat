CALL "C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\vcvarsall.bat" x86

xsd.exe "..\FileSchema10.xsd" "..\MidiSchema10.xsd" "DocumentSchema10.xsd" /c /n:CannedBytes.Midi.SpeechController.Serialization.Version10

pause