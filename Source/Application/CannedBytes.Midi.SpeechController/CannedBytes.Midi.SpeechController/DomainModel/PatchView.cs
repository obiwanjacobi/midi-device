using System.Collections.ObjectModel;

namespace CannedBytes.Midi.SpeechController.DomainModel
{
    public class PatchView : KeyedCollection<string, Patch>
    {
        protected override string GetKeyForItem(Patch item)
        {
            return item.Text;
        }
    }
}