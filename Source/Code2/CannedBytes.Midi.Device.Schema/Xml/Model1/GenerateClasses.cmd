CALL "C:\Program Files (x86)\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86

xsd MidiDeviceSchema.xsd /c /f /order /n:CannedBytes.Midi.Device.Schema.Xml.Model1


@ECHO WARNING!!!!!
@ECHO .
@ECHO Replace public localField[] sequence;  with   public fieldSequence sequence; at:
@ECHO .  
@ECHO public partial class recordExtensionType : annotated
@ECHO                 - AND -
@ECHO public partial class recordRestrictionType : annotated
@ECHO . 
@ECHO [System.Xml.Serialization.XmlElementAttribute("sequence", typeof(fieldSequence), Order = 0)]
@ECHO public localField[] sequence;

pause