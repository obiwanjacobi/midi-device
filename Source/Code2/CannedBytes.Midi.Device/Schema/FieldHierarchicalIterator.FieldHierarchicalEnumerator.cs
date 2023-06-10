﻿using CannedBytes.Collections;
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
                FieldInfo fieldInfo = Current;

                if (fieldInfo.Field?.RecordType != null)
                {
                    return ToFieldInfos(fieldInfo.Field.RecordType.Fields).GetEnumerator();
                }

                return null;
            }
        }
    }
}
