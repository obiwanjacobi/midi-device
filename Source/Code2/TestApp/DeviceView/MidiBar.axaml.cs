using Avalonia.Controls;

namespace TestApp.DeviceView;

public partial class MidiBar : UserControl
{
    public MidiBar()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var deviceModel = DataContext as DeviceViewModel;
        if (deviceModel is not null)
            DataContext = new MidiViewModel(deviceModel);
    }
}
