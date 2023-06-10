namespace CannedBytes.Midi.Device.Converters;

public interface IDataConverterExtension
{
    DataConverter InnerConverter { get; set; }
}
