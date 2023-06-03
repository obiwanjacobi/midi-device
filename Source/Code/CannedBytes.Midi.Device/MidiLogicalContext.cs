namespace CannedBytes.Midi.Device
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CannedBytes.Midi.Device.Schema;

    /// <summary>
    /// The MidiLogicalContext class provides the application component that consumes or provides
    /// the logical midi device schema data with context information on each call that is made.
    /// </summary>
    public class MidiLogicalContext
    {
        public MidiLogicalContext(RecordType rootType, Field field, IEnumerable<FieldConverterEnumerator> fieldConverterEnums)
        {
            Check.IfArgumentNull(rootType, "rootType");
            Check.IfArgumentNull(field, "field");
            Check.IfArgumentNull(fieldConverterEnums, "fieldConverterEnums");

            RootRecordType = rootType;
            Field = field;
            Key = new FieldPathKey();

            List<FieldInfo> infos = new List<FieldInfo>();
            foreach (FieldConverterEnumerator fieldConvEnum in fieldConverterEnums)
            {
                Key.Add(fieldConvEnum.InstanceIndex);

                infos.Add(new FieldInfo(fieldConvEnum));
            }

            FieldInfos = infos;
        }

        public MidiLogicalContext(RecordType rootType, Field field, IEnumerable<FieldInfo> fieldInfos)
        {
            Check.IfArgumentNull(rootType, "rootType");
            Check.IfArgumentNull(field, "field");
            Check.IfArgumentNull(fieldInfos, "fieldInfos");

            RootRecordType = rootType;
            Field = field;
            Key = new FieldPathKey();

            foreach (var fieldInfo in fieldInfos)
            {
                Key.Add(fieldInfo.InstanceIndex);
            }

            FieldInfos = fieldInfos;
        }

        public RecordType RootRecordType { get; protected set; }

        public Field Field { get; protected set; }

        public FieldPathKey Key { get; protected set; }

        public IEnumerable<FieldInfo> FieldInfos { get; protected set; }

        public class FieldInfo
        {
            internal FieldInfo(FieldConverterEnumerator fieldConvEnum)
            {
                Field = fieldConvEnum.Current.Field;
                InstanceIndex = fieldConvEnum.InstanceIndex;
            }

            public FieldInfo(Field field, int instanceIndex)
            {
                Field = field;
                InstanceIndex = instanceIndex;
            }

            public Field Field { get; private set; }

            public int InstanceIndex { get; private set; }
        }
    }
}