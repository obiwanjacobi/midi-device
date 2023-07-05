using System;
using System.Collections.Generic;
using System.Text;

namespace CannedBytes.Midi.Device;

/// <summary>
/// Represents all the instance indexes from a specific field up to its root.
/// </summary>
/// <remarks>
/// This key uniquely identifies a field in a multi-occurrence collection of hierarchical data.
/// Because the instance index of each parent is specified it identifies the unique position of the Field.
/// </remarks>
public struct InstancePathKey : IEquatable<InstancePathKey>
{
    private const char SeparatorChar = '|';
    private readonly List<int> _values;

    public InstancePathKey()
    {
        _values = new List<int>();
    }

    public InstancePathKey(int instanceIndex)
        : this()
    {
        Add(instanceIndex);
    }

    public InstancePathKey(IEnumerable<int> values)
    {
        _values = new List<int>(values);
    }

    public void Add(int instanceIndex)
    {
        _values.Add(instanceIndex);
    }

    public void AddRange(IEnumerable<int> instanceIndexes)
    {
        _values.AddRange(instanceIndexes);
    }

    public override bool Equals(object? obj)
    {
        if (obj is InstancePathKey key)
        {
            return Equals(key);
        }

        return base.Equals(obj);
    }

    public readonly bool Equals(InstancePathKey key)
    {
        if (_values.Count == key._values.Count)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (_values[i] != key._values[i])
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public override readonly int GetHashCode()
    {
        return _values.GetHashCode();
    }

    /// <summary>
    /// Indicates if all indexes are zero (true).
    /// </summary>
    public readonly bool IsZero
    {
        get
        {
            foreach (int index in _values)
            {
                if (index > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public int Depth
    {
        get { return _values.Count; }
    }

    public IEnumerable<int> Values
    {
        get { return _values; }
    }

    public override string ToString()
    {
        StringBuilder text = new();

        var indices = new List<int>(_values);
        indices.Reverse();

        foreach (int index in indices)
        {
            if (text.Length > 0)
            {
                text.Append(SeparatorChar);
            }

            text.Append(index);
        }

        return text.ToString();
    }

    public static InstancePathKey Parse(string text)
    {
        var values = text.Split(SeparatorChar);
        InstancePathKey result = new();

        foreach (string value in values)
        {
            int index = int.Parse(value);

            result.Add(index);
        }

        return result;
    }
}
