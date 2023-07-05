using Avalonia.Controls;
using TestApp.MainView;

namespace TestApp.SchemaView;

public partial class SchemaView : UserControl
{
    public SchemaView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var mainModel = DataContext as MainViewModel;
        if (mainModel is not null)
            DataContext = new SchemaViewModel(mainModel);
    }
}
