using System;
using System.Text;

namespace CannedBytes.Midi.Console
{
    static class RandomNameGenerator
    {
        private static Random _rnd = new Random(DateTime.Now.Millisecond);

        public static string CreateName(int minChars, int maxChars)
        {
            var length = DetermineLength(minChars, maxChars);
            var text = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                text.Append((char)_rnd.Next(' ', 'z'));
            }

            return text.ToString();
        }

        private static int DetermineLength(int minChars, int maxChars)
        {
            return _rnd.Next(minChars, maxChars);
        }
    }
}