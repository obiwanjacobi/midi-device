namespace CannedBytes.Midi.SpeechController.Service
{
    internal interface IBinaryTextService
    {
        byte[] Parse(string binaryText);
    }
}