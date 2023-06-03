using System.Collections.Generic;
using CannedBytes.Midi.Console.UI.ViewModels.Device;
using CannedBytes.Midi.Console.UI.ViewModels.Schema;

namespace CannedBytes.Midi.Console.UI.ViewModels
{
    class OverviewPageViewModel : PageViewModel
    {
        public OverviewPageViewModel()
        {
            DisplayName = "Overview";

            List<ViewModel> contentTypes = new List<ViewModel>();

            contentTypes.Add(new DeviceInstancesViewModel());
            //contentTypes.Add(new PatchInstancesViewModel());
            contentTypes.Add(new SchemaInstancesViewModel());

            ContentTypes = contentTypes;
        }

        /// <summary>
        /// Gets the view models for the root content types (Devices, Schemas, Patches).
        /// </summary>
        public IEnumerable<ViewModel> ContentTypes { get; private set; }

        public T FindContentType<T>() where T : ViewModel
        {
            foreach (var contentType in ContentTypes)
            {
                var item = contentType as T;

                if (item != null) return item;
            }

            return null;
        }
    }
}