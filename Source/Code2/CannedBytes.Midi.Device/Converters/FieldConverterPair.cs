using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Converters;

/// <summary>
/// The FieldConverterPair class stores a <see cref="Field"/> with its matching <see cref="IConverter"/> instance.
/// </summary>
public readonly struct FieldConverterPair
{
    /// <summary>
    /// Constructs a new fully initialized instance.
    /// </summary>
    /// <param name="field">Must not be null.</param>
    /// <param name="converter">Must not be null.</param>
    public FieldConverterPair(Field field, IConverter converter)
    {
        Assert.IfArgumentNull(field, nameof(field));
        Assert.IfArgumentNull(converter, nameof(converter));

        Field = field;
        Converter = converter;
    }

    /// <summary>
    /// Gets the field.
    /// </summary>
    public Field Field { get; }

    /// <summary>
    /// Gets the converter.
    /// </summary>
    public IConverter Converter { get; }

    public DataConverter DataConverter
    {
        get { return Converter as DataConverter; }
    }

    public StreamConverter StreamConverter
    {
        get { return Converter as StreamConverter; }
    }

    public override string ToString()
    {
        return $"{Field.ToString()} - {Converter.ToString()}";
    }
}
