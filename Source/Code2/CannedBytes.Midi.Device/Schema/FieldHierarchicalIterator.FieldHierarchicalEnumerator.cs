using CannedBytes.Collections;
using System.Collections.Generic;

namespace CannedBytes.Midi.Device.Schema
{
    partial class FieldHierarchicalIterator
    {
        private class FieldHierarchicalEnumerator : HierarchicalEnumerator<FieldInfo>
        {
            public FieldHierarchicalEnumerator(IEnumerable<FieldInfo> fields)
                : base(fields)
            { }

            protected override IEnumerator<FieldInfo> GetChildEnumerator()
            {
                var fieldInfo = Current;

                if (fieldInfo.Field != null &&
                    fieldInfo.Field.RecordType != null)
                {
                    return FieldHierarchicalIterator.ToFieldInfos(fieldInfo.Field.RecordType.Fields).GetEnumerator();
                }

                return null;
            }
        }
    }
}
