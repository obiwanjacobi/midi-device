using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using CannedBytes.Midi.Device;
using CannedBytes.Midi.Device.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace TestApp.DeviceView
{
    internal partial class DeviceViewModel : ViewModel
    {
        // designer support
        public DeviceViewModel()
        {
            SchemaNames = new[] { "Schema1", "Schema2" };
            SelectedSchemaName = "Schema1";
        }

        public DeviceViewModel(ViewModel viewModel)
            : base(viewModel)
        {
            _schemaProvider = viewModel.Services.GetRequiredService<IDeviceSchemaProvider>();

            SchemaNames = _schemaProvider.SchemaNames;
            SelectedSchemaName = SchemaNames.First();
        }

        private readonly IDeviceSchemaProvider _schemaProvider;
        private DeviceProvider? _deviceProvider;
        public IEnumerable<string> SchemaNames { get; }

        [ObservableProperty]
        private string _selectedSchemaName;
        partial void OnSelectedSchemaNameChanged(string value)
        {
            if (_schemaProvider is not null)
            {
                var schemaName = SchemaName.FromSchemaNamespace(SelectedSchemaName);
                _deviceProvider = DeviceProvider.Create(Services, schemaName);

                SysExMessageTypes = _deviceProvider.Schema.VirtualRootFields
                    .Select(f => f.Name.Name)
                    .ToList();
            }
        }

        [ObservableProperty]
        private IEnumerable<string> _sysExMessageTypes;
        [ObservableProperty]
        private string _selectedMessageType;
        partial void OnSelectedMessageTypeChanged(string value)
        {
            var virtualField = _deviceProvider!.Schema.VirtualRootFields
                .Single(vf => vf.Name.Name == value);

            var binMap = _deviceProvider.GetBinaryConverterMapFor(virtualField);
            var nodes = binMap.RootNode.SelectNodes(n => n.Next);
            MessageFields = new HierarchicalTreeDataGridSource<SchemaNode>(nodes)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<SchemaNode>(
                        new TextColumn<SchemaNode, string>("Field", n => n.Field.Name.Name), n => Enumerable.Empty<SchemaNode>()),
                    new TextColumn<SchemaNode, string>("Type", n => n.Field.RecordType != null ? n.Field.RecordType.Name.Name : n.Field.DataType.Name.Name),
                    new TextColumn<SchemaNode, string>("Path", n => GetNodeInstancePath(n)),
                    new TemplateColumn<SchemaNode>("Value", new FuncDataTemplate<SchemaNode>((schemaNode, nameScope) => new TextBox()))
                }
            };
        }
        
        private static string GetNodeInstancePath(SchemaNode schemaNode)
            => schemaNode.Key.ToString();

        [ObservableProperty]
        private HierarchicalTreeDataGridSource<SchemaNode> _messageFields;
    }
}
