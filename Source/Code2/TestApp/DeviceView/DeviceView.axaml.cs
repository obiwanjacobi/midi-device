using Avalonia.Controls;
using TestApp.MainView;

namespace TestApp.DeviceView;

public partial class DeviceView : UserControl
{
    public DeviceView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var mainModel = DataContext as MainViewModel;
        if (mainModel is not null)
            DataContext = new DeviceViewModel(mainModel);
    }
}
