using System;

namespace CannedBytes.Midi.Device;

partial class DeviceToLogicalProcess
{
    /// <summary>
    /// This class iterates the nodes in the schema and calls each converter ToLogical method.
    /// </summary>
    public sealed class ProcessToLogical
    {
        private readonly LogicalDeviceDataContext _context;
        private readonly SchemaNode _rootNode;

        public ProcessToLogical(LogicalDeviceDataContext context, SchemaNode rootNode)
        {
            _context = context;
            _rootNode = rootNode;
        }

        public SchemaNode CurrentNode
        {
            get { return _context.FieldInfo.CurrentNode; }
            set { _context.FieldInfo.CurrentNode = value; }
        }

        private LogicalWriteAccessor _accessor;
        public void ToLogical(IMidiLogicalWriter writer)
        {
            _accessor = new LogicalWriteAccessor(_context, writer);

            ToLogical(_rootNode);
        }

        private void ToLogical(SchemaNode thisNode)
        {
            if (thisNode.IsRecord)
            {
                RecordToLogical(thisNode);
            }
            else
            {
                FieldToLogical(thisNode);
            }
        }

        private void RecordToLogical(SchemaNode thisNode)
        {
            using IDisposable scope = _context.LogManager.BeginNewEntry();
            try
            {
                OnBeforeRecordToLogical(thisNode);
                OnRecordToLogical(thisNode);
                OnAfterRecordToLogical(thisNode);
            }
            catch (Exception e)
            {
                _context.LogManager.SetError(e);

                throw;
            }
        }

        private void FieldToLogical(SchemaNode thisNode)
        {
            using IDisposable scope = _context.LogManager.BeginNewEntry();
            try
            {
                OnBeforeFieldToLogical(thisNode);
                OnFieldToLogical(thisNode);
                OnAfterFieldToLogical(thisNode);
            }
            catch (Exception e)
            {
                _context.LogManager.SetError(e);

                throw;
            }
        }

        private void OnBeforeRecordToLogical(SchemaNode thisNode)
        {
            ClearCarry();
            CurrentNode = thisNode;

            var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnBeforeRecord(_context);
        }

        private void OnRecordToLogical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;

            var iterator = thisNode.FieldConverterPair.StreamConverter.GetChildNodeIterator(_context);

            foreach (SchemaNode node in iterator)
            {
                ToLogical(node);
            }
        }

        private void OnAfterRecordToLogical(SchemaNode thisNode)
        {
            var navEvents = thisNode.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnAfterRecord(_context);

            // auto pop stream from the stack
            _context.StreamManager.RemoveCurrentStream(thisNode.FieldConverterPair.StreamConverter);

            ClearCarry();
        }

        private void OnBeforeFieldToLogical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;
            var navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnBeforeField(_context);
        }

        private void OnFieldToLogical(SchemaNode thisNode)
        {
            CurrentNode = thisNode;

            var reader = new DeviceStreamReader(
                _context.StreamManager.CurrentStream, _context.BitReader);

            thisNode.FieldConverterPair.DataConverter.ToLogical(_context, reader, _accessor);
        }

        private void OnAfterFieldToLogical(SchemaNode thisNode)
        {
            var navEvents = thisNode.Parent.FieldConverterPair.StreamConverter as INavigationEvents;

            navEvents?.OnAfterField(_context);
        }

        private void ClearCarry()
        {
            _context.BitReader.Clear();

            if (_context.LogManager.CurrentEntry != null)
            {
                _context.LogManager.CurrentEntry.CarryCleared = true;
            }
        }
    }
}