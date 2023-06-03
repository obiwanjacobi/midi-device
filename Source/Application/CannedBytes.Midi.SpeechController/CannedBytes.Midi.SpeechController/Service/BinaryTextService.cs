using System;
using System.Collections.Generic;
using System.Globalization;

namespace CannedBytes.Midi.SpeechController.Service
{
    internal class BinaryTextService : IBinaryTextService
    {
        #region IBinaryTextService Members

        public byte[] Parse(string binaryText)
        {
            List<byte> bytes = new List<byte>();
            string[] byteParts = binaryText.Split(' ', ',', ';');

            foreach (string part in byteParts)
            {
                bytes.Add(Byte.Parse(part, NumberStyles.HexNumber));
            }

            return bytes.ToArray();
        }

        #endregion IBinaryTextService Members
    }
}