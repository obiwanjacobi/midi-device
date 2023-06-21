namespace CannedBytes.Midi.Device;

partial class DeviceToLogicalProcess
{
    /// <summary>
    /// This class iterates the nodes in the schema and calls each converter ToLogical method.
    /// </summary>
    public sealed class ProcessToLogical : DeviceProcess<LogicalDeviceDataContext>
    {
        private LogicalWriteAccessor _accessor;

        public ProcessToLogical(LogicalDeviceDataContext context, SchemaNode rootNode)
            : base(context, rootNode)
        { }
        
        public void ToLogical(IMidiLogicalWriter writer)
        {
            _accessor = new LogicalWriteAccessor(Context, writer);

            Execute(RootNode);
        }

        protected override void OnBeforeRecord(SchemaNode thisNode)
        {
            ClearCarry();
            base.OnBeforeRecord(thisNode);
        }

        protected override void OnField(SchemaNode thisNode)
        {
            var reader = new DeviceStreamReader(
                Context.StreamManager.CurrentStream, Context.BitReader);

            thisNode.FieldConverterPair.DataConverter.ToLogical(Context, reader, _accessor);
        }

        private void ClearCarry()
        {
            Context.BitReader.Clear();
            LogCarryCleared();
        }
    }
}