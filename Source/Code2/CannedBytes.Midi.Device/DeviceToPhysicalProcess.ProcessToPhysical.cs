namespace CannedBytes.Midi.Device;

partial class DeviceToPhysicalProcess
{
    /// <summary>
    /// This class iterates the nodes in the schema and calls each converter ToPhysical method.
    /// </summary>
    public sealed class ProcessToPhysical : DeviceProcess<PhysicalDeviceDataContext>
    {
        private LogicalReadAccessor? _accessor;

        public ProcessToPhysical(PhysicalDeviceDataContext context, SchemaNode rootNode)
            : base(context, rootNode)
        { }

        public void ToPhysical(IMidiLogicalReader reader)
        {
            _accessor = new LogicalReadAccessor(Context, reader);

            Execute(RootNode);
        }

        protected override void OnAfterRecord(SchemaNode thisNode)
        {
            FlushCarry();
            base.OnAfterRecord(thisNode);
        }

        protected override void OnField(SchemaNode thisNode)
        {
            var writer = new DeviceStreamWriter(
                Context.StreamManager.CurrentStream, Context.BitWriter);

            thisNode.FieldConverterPair.DataConverter!.ToPhysical(Context, writer, _accessor!);
        }

        private void FlushCarry()
        {
            Context.BitWriter.Flush(Context.StreamManager.CurrentStream);
            LogCarryCleared();
        }
    }
}