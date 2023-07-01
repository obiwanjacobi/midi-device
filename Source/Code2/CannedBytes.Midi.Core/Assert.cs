using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CannedBytes.Midi.Core
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void IfArgumentNull<T>(T? argument, string argumentName,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
            where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(argumentName, 
                    $"Argument '{argumentName}' was null at {caller} ({line}).");
            }
        }

        [Conditional("DEBUG")]
        public static void IfArgumentNull<T>(T? argument, string argumentName, string message,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
            where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(argumentName, 
                    $"{message} for '{argumentName}' at {caller} ({line}).");
            }
        }

        [Conditional("DEBUG")]
        public static void IfArgumentNullOrEmpty(string? argument, string argumentName,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
        {
            if (String.IsNullOrEmpty(argument))
            {
                IfArgumentNull(argument, argumentName, caller, line);
                throw new ArgumentException(
                    $"The specified string for '{argumentName}' is empty at {caller} ({line}).", 
                    argumentName);
            }
        }

        [Conditional("DEBUG")]
        public static void IfArgumentOutOfRange<T>(IComparable<T> argument, T minValue, T maxValue, string argumentName,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
            where T : struct
        {
            if (argument.CompareTo(minValue) < 0 || argument.CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argument,
                    $"The value '{argument}' for '{argumentName}' is expected to lie between {minValue} and {maxValue} at {caller} ({line}).");
            }
        }

        [Conditional("DEBUG")]
        public static void IfArgumentTooLong(string argument, int maxLength, string argumentName,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
        {
            if (argument is not null && argument.Length > maxLength)
            {
                throw new ArgumentException(
                    $"The value '{argument}' for '{argumentName}' exceeds the maximum of {maxLength} at {caller} ({line}).",
                    argumentName);
            }
        }

        [Conditional("DEBUG")]
        public static void IfArgumentNotOfType<T>(object? argument, string argumentName,
            [CallerMemberName] string? caller = null, [CallerLineNumber] int line = default)
        {
            if (!(argument is T))
            {
                throw new ArgumentException(
                    $"The type '{argument}' of '{argumentName}' is not {typeof(T).FullName} at {caller} ({line}).",
                    argumentName);
            }
        }
    }
}
