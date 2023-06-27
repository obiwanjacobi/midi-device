using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CannedBytes.Midi.Device.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TestApp.MainView;

namespace TestApp.SchemaView;

internal partial class SchemaViewModel : ViewModel
{
    private readonly IDeviceSchemaProvider _schemaProvider;

    // designer support
    public SchemaViewModel()
    {
        Roots = new HierarchicalTreeDataGridSource<Field>(Enumerable.Empty<Field>());
        SchemaNames = new[] { "Schema1", "Schema2" };
        SelectedSchemaName = "Schema1";
    }

    public SchemaViewModel(MainViewModel mainModel)
    {
        _schemaProvider = mainModel.Services.GetRequiredService<IDeviceSchemaProvider>();
        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland A-880.mds"));
        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland FC-300.mds"));
        _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland U-220.mds"));
        //_schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland R-8.mds"));
        var deviceSchema = _schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland D-110.mds"));

        SchemaNames = _schemaProvider.SchemaNames;
        SelectedSchemaName = deviceSchema.Name.SchemaName;
    }

    public IEnumerable<string> SchemaNames { get; }

    [ObservableProperty]
    private Field? _selectedField;
    [ObservableProperty]
    private IEnumerable<KeyValuePair<string, string>> _fieldProperties;

    [ObservableProperty]
    private HierarchicalTreeDataGridSource<Field>? _roots;

    [ObservableProperty]
    private string _selectedSchemaName;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (_schemaProvider != null &&
            e.PropertyName == nameof(SelectedSchemaName))
        {
            var deviceSchema = _schemaProvider.Open(SchemaName.FromSchemaNamespace(SelectedSchemaName));
            Roots = FillTree(deviceSchema);
            // add selection changed trigger
            Roots.RowSelection.PropertyChanged += SchemaRowSelection_PropertyChanged;
        }
    }

    private void SchemaRowSelection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HierarchicalTreeDataGridSource<Field>.RowSelection.SelectedItem))
        {
            SelectedField = Roots!.RowSelection?.SelectedItem;
            FieldProperties = FillFieldProperties(SelectedField!);
        }
    }

    private IEnumerable<KeyValuePair<string, string>> FillFieldProperties(Field field)
    {
        var props = new Dictionary<string, string>()
        {
            { "Name", field.Name.Name },
            { "Schema", field.Name.SchemaName },
            { "DataType", field.DataType?.Name.Name },
            { "RecordType", field.RecordType?.Name.Name },
            { "IsAbstract", field.IsAbstract.ToString() },
            { "Device Property Name", field.Properties.DevicePropertyName },
            { "Address", field.Properties.Address.ToString() },
            { "Range", field.Properties.Range?.ToString() },
            { "Repeats", field.Properties.Repeats.ToString() },
            { "Size", field.Properties.Size.ToString() },
            { "Width", field.Properties.Width.ToString() },
            { "# Attributes", field.Attributes.Count.ToString() },
            { "# Constraints", field.Constraints.Count.ToString() },
        };

        return props;
    }

    private HierarchicalTreeDataGridSource<Field> FillTree(DeviceSchema deviceSchema)
    {
        var fields = deviceSchema.VirtualRootFields;
        var tree = new HierarchicalTreeDataGridSource<Field>(fields)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<Field>(
                    new TextColumn<Field, string>("Field", f => f.Name.Name), f => f.RecordType?.Fields ?? Enumerable.Empty<Field>()),
                new TextColumn<Field, string>("Type", f => f.RecordType != null ? f.RecordType.Name.Name : f.DataType.Name.Name),
                new TextColumn<Field, string>("Kind", f => f.RecordType != null ? "Record" : "Data"),
                new TextColumn<Field, string>("Schema", f => f.Name.SchemaName)
            }
        };

        return tree;
    }
}
