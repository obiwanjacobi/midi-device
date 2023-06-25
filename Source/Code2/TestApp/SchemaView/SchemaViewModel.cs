using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Schema;
using Microsoft.Extensions.DependencyInjection;
using TestApp.MainView;

namespace TestApp.SchemaView;

internal partial class SchemaViewModel : ViewModelBase
{
    public SchemaViewModel(MainViewModel mainModel)
    {
        var schemaProvider = mainModel.Services.GetRequiredService<IDeviceSchemaProvider>();
        var deviceSchema = schemaProvider.Open(SchemaName.FromAssemblyResource("CannedBytes.Midi.Device.Roland", "Roland D-110.mds"));
        Roots = FillTree(deviceSchema);
    }

    public HierarchicalTreeDataGridSource<Field> Roots { get; }

    private HierarchicalTreeDataGridSource<Field> FillTree(DeviceSchema deviceSchema)
    {
        var fields = deviceSchema.VirtualRootFields
            .Where(f => f.RecordType.BaseType?.Name.FullName == MidiTypes.MidiTypesSchema_SysEx);
        var tree = new HierarchicalTreeDataGridSource<Field>(fields)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<Field>(
                    new TextColumn<Field, string>("Name", f => f.Name.Name),
                    f => f.RecordType?.Fields ?? Enumerable.Empty<Field>()),
                new TextColumn<Field, string>("Type", f => f.RecordType != null ? f.RecordType.Name.Name : f.DataType.Name.Name),
                new TextColumn<Field, string>("Kind", f => f.RecordType != null ? "RecordType" : "DataType"),
                new TextColumn<Field, string>("Schema", f => f.Name.SchemaName)
            }
        };

        return tree;
    }
}
