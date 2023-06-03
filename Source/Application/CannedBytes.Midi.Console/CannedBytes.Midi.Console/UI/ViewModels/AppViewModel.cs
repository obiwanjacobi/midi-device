using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using CannedBytes.Midi.Console.UI.Command;
using CannedBytes.Midi.Console.UI.ViewModels.Device;
using CannedBytes.Midi.Console.UI.ViewModels.Schema;
using CannedBytes.Midi.Console.UI.Views;
using MahApps.Metro.Controls;

namespace CannedBytes.Midi.Console.UI.ViewModels
{
    /// <summary>
    /// Application root view model
    /// </summary>
    [Export]
    class AppViewModel : Conductor<PageViewModel>
    {
        private Stack<PageViewModel> _pageStack = new Stack<PageViewModel>();

        public AppViewModel()
        {
            DisplayName = "Canned Bytes - Midi Console";

            Flyouts = new BindableCollection<ViewModel>();
            Flyouts.Add(AppBarFlyout);
            Flyouts.Add(PropertiesFlyout);

            UserMessages = new BindableCollection<UserMessageViewModel>();

            OverviewPageViewModel = new OverviewPageViewModel();
            _pageStack.Push(OverviewPageViewModel);

            ActivateItem(OverviewPageViewModel);
        }

        public OverviewPageViewModel OverviewPageViewModel { get; private set; }

        public T FindContentType<T>() where T : ViewModel
        {
            return OverviewPageViewModel.FindContentType<T>();
        }

        public BindableCollection<ViewModel> Flyouts { get; private set; }

        [Import]
        public AppBarViewModel AppBarFlyout { get; private set; }

        [Import]
        public PropertiesViewModel PropertiesFlyout { get; private set; }

        public BindableCollection<UserMessageViewModel> UserMessages { get; private set; }

        public void CloseUserMessage(UserMessageViewModel userMsg)
        {
            UserMessages.Remove(userMsg);
        }

        public void ShowHelpMessage()
        {
            UserMessages.Add(new UserMessageViewModel() { DisplayName = "Help", MessageText = "The Help command is not implemented yet." });
        }

        public void NavigateToFilePage()
        {
            HideFlyouts();

            var page = new FilePageViewModel(this);
            ActivateItem(page);
        }

        public void ActivateDevice(DeviceViewModel deviceViewModel)
        {
            HideFlyouts();

            var page = new DevicePageViewModel();
            page.ActiveDevice = deviceViewModel;

            _pageStack.Push(page);

            ActivateItem(page);
        }

        public void EditSchema(SchemaViewModel schemaViewModel)
        {
            HideFlyouts();

            var page = new SchemaEditorPageViewModel();
            page.Schemas = this.OverviewPageViewModel.FindContentType<SchemaInstancesViewModel>().Schemas;
            page.ActiveSchema = schemaViewModel;

            _pageStack.Push(page);

            ActivateItem(page);
        }

        public void NavigateBack()
        {
            HideFlyouts();

            if (_pageStack.Count > 1)
            {
                _pageStack.Pop();
            }

            ActivateItem(_pageStack.Peek());
        }

        private void HideFlyouts()
        {
            AppBarFlyout.IsOpen = false;
            PropertiesFlyout.IsOpen = false;
        }

        public void ShowAppBar(ViewModel vm)
        {
            AppBarFlyout.ActiveViewModel = vm;
            AppBarFlyout.IsOpen = true;
        }

        public void ShowProperties(ViewModel vm)
        {
            PropertiesFlyout.ActiveViewModel = vm;
            PropertiesFlyout.IsOpen = true;
        }

        protected override void OnViewAttached(object view, object context)
        {
            var ctrl = view as Control;

            if (ctrl != null)
            {
                // Add root command handlers
                ctrl.CommandBindings.Add(new RightClickCommandHandler(this).ToCommandBinding());
                ctrl.CommandBindings.Add(new ShowPropertiesCommandHandler(this).ToCommandBinding());
            }

            InitializeMainWindow(view as MetroWindow);

            base.OnViewAttached(view, context);
        }

        private void InitializeMainWindow(MetroWindow metroWnd)
        {
            if (metroWnd != null)
            {
                Flyout flyout = new AppBarView();
                flyout.DataContext = AppBarFlyout;
                flyout.IsOpen = true;
                metroWnd.Flyouts.Add(flyout);

                flyout = new PropertiesView();
                flyout.DataContext = PropertiesFlyout;
                flyout.IsOpen = true;
                metroWnd.Flyouts.Add(flyout);
            }
        }
    }
}