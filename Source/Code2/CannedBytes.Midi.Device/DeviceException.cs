namespace CannedBytes.Midi.Device;

[System.Serializable]
public class DeviceException : System.Exception
{
    public DeviceException() { }
    public DeviceException(string message) : base(message) { }
    public DeviceException(string message, System.Exception inner) : base(message, inner) { }
    protected DeviceException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
