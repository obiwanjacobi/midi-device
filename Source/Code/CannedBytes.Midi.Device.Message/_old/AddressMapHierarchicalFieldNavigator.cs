using System.Collections.Generic;
using System.Diagnostics;
using CannedBytes.Midi.Device.Converters;
using CannedBytes.Midi.Device.Schema;

namespace CannedBytes.Midi.Device.Message
{
    /// <summary>
    /// The AddressMapHierarchicalFieldNavigator class preserves the hierarchy of the RecordType
    /// structure.
    /// </summary>
    internal class AddressMapHierarchicalFieldNavigator : AddressMapSequentialFieldNavigator
    {
        private ConverterManager _converterMgr;

        public AddressMapHierarchicalFieldNavigator(ConverterManager converterMgr, FieldConverterPair rootPair)
            : base(rootPair)
        {
            _converterMgr = converterMgr;
        }

        private Dictionary<string, DynamicFieldConverterPair> _fullPairs = new Dictionary<string, DynamicFieldConverterPair>();

        public IEnumerable<FieldConverterPair> FullGroupConverterPairs
        {
            get { return _fullPairs.Values; }
        }

        public FieldConverterPair StartGroupConverterPair { get; set; }

        public FieldConverterPair EndGroupConverterPair { get; set; }

        private Stack<DynamicFieldConverterPair> _dynamicPairs = new Stack<DynamicFieldConverterPair>();
        private DynamicFieldConverterPair _rootPair;

        public GroupConverter RootGroupConverter
        {
            get
            {
                if (_rootPair == null)
                {
                    return null;
                }

                return (GroupConverter)_rootPair.Converter;
            }
        }

        protected override void AddField(DynamicFieldConverterPair fieldConverterPair)
        {
            base.AddField(fieldConverterPair);

            var currentRecord = CurrentDynamicRecord;

            AddChild(currentRecord, fieldConverterPair);
        }

        protected DynamicFieldConverterPair PushDynamicRecord(FieldConverterPair fieldPair)
        {
            Debug.Assert(fieldPair.Field.RecordType != null, "Field is not a RecordType.");

            var dynField = new DynamicField(fieldPair.Field);
            var converter = _converterMgr.GetConverter(fieldPair.Field.RecordType, dynField.DynamicRecordType);
            var pair = new DynamicFieldConverterPair(dynField, converter, Context.CurrentInstanceIndex, CurrentAddress);

            if (_dynamicPairs.Count == 0 &&
                _rootPair == null)
            {
                _rootPair = pair;
            }

            _dynamicPairs.Push(pair);

            return pair;
        }

        protected DynamicFieldConverterPair PopDynamicRecord(FieldConverterPair groupPair)
        {
            DynamicFieldConverterPair popped = null;

            if (_dynamicPairs.Count == 0)
            {
                if (_rootPair != null &&
                    this.EndConverterPair == null)
                {
                    Field parentField = null;

                    if (this.FieldGroupConverterStack.Count > 0)
                    {
                        FieldConverterPair parentPair = this.FieldGroupConverterStack.Peek();
                        parentField = parentPair.Field;
                    }
                    else
                    {
                        parentField = Field.VirtualRootField;
                    }

                    var dynField = new DynamicField(parentField);
                    var converter = _converterMgr.GetConverter(parentField.RecordType, dynField.DynamicRecordType);
                    var pair = new DynamicFieldConverterPair(dynField, converter, Context.CurrentInstanceIndex, CurrentAddress);

                    AddChild(pair, _rootPair);

                    popped = _rootPair;
                    _rootPair = pair;
                }
            }
            else
            {
                if (groupPair.Field.Name.FullName == CurrentDynamicRecord.Field.Name.FullName)
                {
                    popped = _dynamicPairs.Pop();

                    // this record has fields assigned, link it up to its parent.
                    if (popped.Field.RecordType.Fields.Count > 0)
                    {
                        var parentRec = CurrentDynamicRecord;

                        if (parentRec == null &&
                            !object.ReferenceEquals(popped, _rootPair))
                        {
                            parentRec = _rootPair;
                        }

                        AddChild(parentRec, popped);
                    }
                }
                else
                {
                    Debugger.Break();
                }
            }

            return popped;
        }

        private static bool AddChild(FieldConverterPair parent, FieldConverterPair child)
        {
            if (object.ReferenceEquals(parent, child))
            {
                return false;
            }

            if (parent != null)
            {
                ((DynamicField)parent.Field).DynamicRecordType.AddField(child.Field);
                ((GroupConverter)parent.Converter).FieldConverterMap.Add(child);

                return true;
            }

            return false;
        }

        protected FieldConverterPair CurrentDynamicRecord
        {
            get
            {
                if (_dynamicPairs.Count == 0)
                {
                    return null;
                }

                return _dynamicPairs.Peek();
            }
        }

        protected override NavigationResult OnFieldGroupConverter(FieldConverterPair fieldGroupConverterPair)
        {
            bool addAsFull = this.StartConverterPair != null && this.EndConverterPair == null;

            PushDynamicRecord(fieldGroupConverterPair);

            NavigationResult result = base.OnFieldGroupConverter(fieldGroupConverterPair);

            if (addAsFull && this.EndConverterPair == null)
            {
                var dynamicPair = fieldGroupConverterPair as DynamicFieldConverterPair;

                if (dynamicPair == null)
                {
                    dynamicPair = new DynamicFieldConverterPair(fieldGroupConverterPair.Field, fieldGroupConverterPair.Converter,
                                                Context.CurrentInstanceIndex, CurrentAddress);
                }

                _fullPairs.Add(fieldGroupConverterPair.Field.Name.FullName + Context.CurrentInstanceIndex, dynamicPair);
            }

            PopDynamicRecord(fieldGroupConverterPair);

            return result;
        }

        protected override NavigationResult OnFieldConverter(FieldConverterPair fieldConverterPair)
        {
            NavigationResult result = base.OnFieldConverter(fieldConverterPair);

            if (this.StartConverterPair != null &&
                this.StartGroupConverterPair == null)
            {
                this.StartGroupConverterPair = this.FieldGroupConverterStack.Peek();
            }

            if (this.EndConverterPair != null &&
                this.EndGroupConverterPair == null)
            {
                this.EndGroupConverterPair = this.FieldGroupConverterStack.Peek();
            }

            return result;
        }
    }
}