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
public class InstancePathKey : IEquatable<InstancePathKey>
{
    private const char SeparatorChar = '|';
    private readonly List<int> values;

    public InstancePathKey()
    {
        this.values = new List<int>();
    }

    public InstancePathKey(int instanceIndex)
        : this()
    {
        this.Add(instanceIndex);
    }

    public InstancePathKey(IEnumerable<int> values)
    {
        this.values = new List<int>(values);
    }

    public void Add(int instanceIndex)
    {
        this.values.Add(instanceIndex);
    }

    public void AddRange(IEnumerable<int> instanceIndexes)
    {
        this.values.AddRange(instanceIndexes);
    }

    public override bool Equals(object obj)
    {
        InstancePathKey key = obj as InstancePathKey;

        if (key != null)
        {
            return Equals(key);
        }

        return base.Equals(obj);
    }

    public bool Equals(InstancePathKey key)
    {
        if (this.values.Count == key.values.Count)
        {
            for (int i = 0; i < this.values.Count; i++)
            {
                if (this.values[i] != key.values[i])
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return this.values.GetHashCode();
    }

    /// <summary>
    /// Indicates if all indexes are zero (true).
    /// </summary>
    public bool IsZero
    {
        get
        {
            foreach (int index in this.values)
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
        get { return this.values.Count; }
    }

    public IEnumerable<int> Values
    {
        get { return this.values; }
    }

    public override string ToString()
    {
        StringBuilder text = new();

        foreach (int index in this.values)
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
        string[] values = text.Split(SeparatorChar);
        InstancePathKey result = new();

        foreach (string value in values)
        {
            int index = int.Parse(value);

            result.Add(index);
        }

        return result;
    }
}
