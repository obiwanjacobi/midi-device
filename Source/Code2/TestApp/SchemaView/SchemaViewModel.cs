using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CannedBytes.Midi.Device.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TestApp.MainView;

namespace TestApp.SchemaView;

internal partial class SchemaViewModel : ViewModelBase
{
    private readonly IDeviceSchemaProvider _schemaProvider;

    // designer support
    public SchemaViewModel()
    {
        SchemaNames = new ObservableCollection<string>(new[] { "Schema1", "Schema2" });
        Roots = new HierarchicalTreeDataGridSource<Field>(Enumerable.Empty<Field>());
        SelectedDeviceSchema = "Schema1";
    }

    public SchemaViewModel(MainViewModel mainModel)
    {
        _schemaProvider = mainModel.Services.GetRequiredService<IDeviceSchemaProvider>();
        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland A-880.mds"));
        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland FC-300.mds"));
        //        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland R-8.mds"));
        //        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland U-220.mds"));

        var deviceSchema = _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland D-110.mds"));

        SchemaNames = new ObservableCollection<string>(_schemaProvider.SchemaNames);

        SelectedDeviceSchema = deviceSchema.Name.SchemaName;
    }

    public ObservableCollection<string> SchemaNames { get; }

    [ObservableProperty]
    private HierarchicalTreeDataGridSource<Field>? _roots;

    [ObservableProperty]
    private string _selectedDeviceSchema;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedDeviceSchema))
        {
            var deviceSchema = _schemaProvider.Open(SchemaName.FromSchemaNamespace(SelectedDeviceSchema));
            Roots = FillTree(deviceSchema);
        }
    }

    private HierarchicalTreeDataGridSource<Field> FillTree(DeviceSchema deviceSchema)
    {
        var fields = deviceSchema.VirtualRootFields;
        var tree = new HierarchicalTreeDataGridSource<Field>(fields)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<Field>(
                    new TextColumn<Field, string>("Name", f => f.Name.Name),
                    f => f.RecordType?.Fields ?? Enumerable.Empty<Field>()),
                new TextColumn<Field, string>("Type", f => f.RecordType != null ? f.RecordType.Name.Name : f.DataType.Name.Name),
                new TextColumn<Field, string>("Kind", f => f.RecordType != null ? "Record" : "Data"),
                new TextColumn<Field, string>("Schema", f => f.Name.SchemaName)
            }
        };

        return tree;
    }
}
