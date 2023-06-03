using Caliburn.Micro;

namespace CannedBytes.Midi.Console.UI.ViewModels.Patch
{
    class PatchInstancesViewModel : ViewModel
    {
        public PatchInstancesViewModel()
        {
            DisplayName = "Patches";
            Patches = new BindableCollection<PatchViewModel>();

            AddPatch("dfsdfsd");
            AddPatch("cdcdwef");
            AddPatch("reyqwbqb qgt");
            AddPatch("qer q regert");
            AddPatch("rgnxrass45y");
            AddPatch("vg barsgq");
            AddPatch("dfbaqtrjuy7ws");
            AddPatch("dafaerth rt54hsrb");
            AddPatch("swbfb");
            AddPatch("rebn wtnbw");
            AddPatch("fbst5srtstrh");
            AddPatch("sdfb rbt");
            AddPatch("dxf bts4t");
            AddPatch("erg fwsrtw");
        }

        private void AddPatch(string name)
        {
            var patch = new PatchViewModel();
            patch.DisplayName = name;

            Patches.Add(patch);
        }

        private BindableCollection<PatchViewModel> _patches;

        public BindableCollection<PatchViewModel> Patches
        {
            get
            {
                return _patches;
            }

            set
            {
                if (_patches != value)
                {
                    _patches = value;
                    NotifyOfPropertyChange(() => Patches);
                }
            }
        }
    }
}