using System;

namespace CannedBytes.Midi.Device;

partial class DeviceToPhysicalProcess
{
    /// <summary>
    /// This class iterates the nodes in the schema and calls each converter ToPhysical method.
    /// </summary>
    public sealed class ProcessToPhysical
    {
        private readonly PhysicalDeviceDataContext _context;
        private readonly SchemaNode _rootNode;

        public ProcessToPhysical(PhysicalDeviceDataContext context, SchemaNode rootNode)
        {
            _context = context;
            _rootNode = rootNode;
        }

        public SchemaNode CurrentNode
        {
            get { return _context.FieldInfo.CurrentNode; }
            set { _context.FieldInfo.CurrentNode = value; }
        }

        private LogicalReadAccessor _accessor;

        public void ToPhysical(IMidiLogicalReader reader)
        {
            _accessor = new LogicalReadAccessor(_context, reader);

            ToPhysical(_rootNode);
        }

        private void ToPhysical(SchemaNode thisNode)
        {
            if (thisNode.IsRecord)
            {
                RecordToPhysical(thisNode);
            }
            else
            {
                FieldToPhysical(thisNode);
            }
        }

        private void RecordToPhysical(SchemaNode thisNode)
        {
            using IDisposable scope = _context.LogManager.BeginNewEntry();
            try
            {
                OnBeforeRecordToPhysical(thisNode);
                OnRecordToPhysical(thisNode);
                OnAfterRecordToPhysical(thisNode);
            }
            catch (Exception e)
            {
                _context.LogManager.SetError(e);

                throw;
            }
        }

        private void FieldToPhysical(SchemaNode thisNode)
        {
            using IDisposable scope = _context.LogManager.BeginNewEntry();
            try
            {
                OnBeforeFieldToPhysical(thisNode);
                OnFieldToPhysical(thisNode);
                OnAfterFieldToPhysical(thisNode);
            }
            catch (Exception e)
            {
                _context.LogManager.SetError(e);

                throw;
            }
        }

        private void OnBeforeRecordToPhysical(SchemaNode thisNode)
        {
            ClearCarry();
            CurrentNode = thisNode;

            var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnBeforeRecord(_context);
        }

        private void OnRecordToPhysical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;

            var iterator = thisNode.FieldConverterPair.StreamConverter.GetChildNodeIterator(_context);

            foreach (SchemaNode node in iterator)
            {
                ToPhysical(node);
            }
        }

        private void OnAfterRecordToPhysical(SchemaNode thisNode)
        {
            var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnAfterRecord(_context);

            // auto pop stream from the stack
            _context.StreamManager.RemoveCurrentStream(thisNode.FieldConverterPair.StreamConverter);

            ClearCarry();
        }

        private void OnBeforeFieldToPhysical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;
            var navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnBeforeField(_context);
        }

        private void OnFieldToPhysical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;

            var writer = new DeviceStreamWriter(
                _context.StreamManager.CurrentStream, _context.BitWriter);

            thisNode.FieldConverterPair.DataConverter.ToPhysical(_context, writer, _accessor);
        }

        private void OnAfterFieldToPhysical(SchemaNode thisNode)
        {
            var navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnAfterField(_context);
        }

        private void ClearCarry()
        {
            _context.BitWriter.Clear();

            if (_context.LogManager.CurrentEntry != null)
            {
                _context.LogManager.CurrentEntry.CarryCleared = true;
            }
        }
    }
}