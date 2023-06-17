using System;

namespace CannedBytes.Midi.Core
{
    public readonly struct ValueRange
    {
        public const char Separator = ':';

        public ValueRange()
        {
            Start = -1;
            End = -1;
        }

        public ValueRange(string rangeText)
            : this()
        {
            if (!String.IsNullOrWhiteSpace(rangeText))
            {
                var (start, end) = Parse(rangeText.Trim());
                Start = start;
                End = end;
            }
        }

        public bool IsEmpty => Start == -1 && End == -1;

        public int Start { get; }
        public int End { get; }

        public int Length => IsEmpty ? 0 : End + 1 - Start;

        public override string ToString()
        {
            return IsEmpty ? "[empty]" : $"{Start}{Separator}{End}";
        }

        private static (int start, int end) Parse(string rangeText)
        {
            int start = 0;
            int end = Int32.MaxValue;

            int index = rangeText.IndexOf(Separator);
            if (index == -1)
                throw new FormatException("Missing range operator ':'.");

            if (index > 0)
            {
                start = Int32.Parse(rangeText[0..index]);
            }

            if (index < rangeText.Length - 1)
            {
                index++;
                end = Int32.Parse(rangeText[index..]);
            }

            return (start, end);
        }
    }
}
