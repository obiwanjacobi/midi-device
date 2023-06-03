using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using CannedBytes.Midi.Device.Schema.Xml.Model1;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaParser
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(deviceSchema));
        private static XmlReaderSettings _settings = new XmlReaderSettings();

        static MidiDeviceSchemaParser()
        {
            _settings.IgnoreComments = true;
            _settings.IgnoreProcessingInstructions = true;
            _settings.IgnoreWhitespace = true;
            _settings.XmlResolver = new XmlResourceResolver();
        }

        private deviceSchema Deserialize(Stream stream)
        {
            var reader = XmlReader.Create(stream);

            if (_serializer.CanDeserialize(reader))
            {
                return (deviceSchema)_serializer.Deserialize(reader);
            }

            return null;
        }

        private MidiDeviceSchemaSet _schemas;

        public MidiDeviceSchemaParser(MidiDeviceSchemaSet schemas)
        {
            Check.IfArgumentNull(schemas, "schemas");

            this._schemas = schemas;
        }

        public DeviceSchema Parse(Stream stream)
        {
            Check.IfArgumentNull(stream, "stream");

            var sourceSchema = Deserialize(stream);

            if (sourceSchema == null)
            {
                throw new ArgumentException("The provided stream could not be parsed into a Midi Device Schema.", "stream");
            }

            _targetSchema = new MidiDeviceSchema();

            ProcessImports(sourceSchema.Items);
            Fill(sourceSchema, _targetSchema);

            //_schemas.Add(_targetSchema);

            return _targetSchema;
        }

        private void ProcessImports(object[] importsAndAnnotations)
        {
            foreach (var item in importsAndAnnotations)
            {
                var import = item as import;

                if (import != null)
                {
                    ProcessImports(import);
                }
            }
        }

        private void ProcessImports(import import)
        {
            if (_schemas.Find(import.schema) == null)
            {
                var stream = OpenSchema(import.name, import.assembly);

                if (stream == null)
                {
                    throw new DeviceSchemaException(
                        String.Format("Failed to load import {0} / {1} ({2}).",
                            import.schema, import.name, import.assembly));
                }

                var parser = new MidiDeviceSchemaParser(_schemas);
                var schema = parser.Parse(stream);

                if (schema != null)
                {
                    _schemas.Add(schema);
                }
            }
        }

        public static Stream OpenSchema(string name, string assemblyName)
        {
            Stream stream = null;

            if (String.IsNullOrEmpty(assemblyName) &&
                !Path.IsPathRooted(name))
            {
                name = Path.Combine(Environment.CurrentDirectory, name);
            }

            if (File.Exists(name))
            {
                stream = File.OpenRead(name);
            }
            else
            {
                Assembly assembly = null;

                if (String.IsNullOrEmpty(assemblyName))
                {
                    assembly = Assembly.GetEntryAssembly();
                }
                else
                {
                    assembly = Assembly.LoadFrom(assemblyName + ".dll");
                }

                if (assembly != null)
                {
                    stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + name);
                }
            }

            return stream;
        }

        private MidiDeviceSchema _targetSchema;
        private string DocumentationAttributeName = "Documentation";
        private List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>> _deferredDataTypes = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>>();
        private List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>> _deferredRecordTypes = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>>();
        private List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>> _deferredFields = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>>();

        protected virtual void Fill(deviceSchema source, MidiDeviceSchema target)
        {
            Check.IfArgumentNull(source, "source");
            Check.IfArgumentNull(target, "target");

            target.SchemaName = source.schema;
            target.SetVersion(source.version);

            Fill(source.Items, target.Attributes);
            Fill(source.Items1, target.AllDataTypes);
            Fill(source.Items1, target.AllRecordTypes);
        }

        private void Fill(IEnumerable<openAttrs> source, SchemaAttributeCollection schemaAttributes)
        {
            if (source != null)
            {
                foreach (var openAttr in source)
                {
                    var annotation = openAttr as annotation;

                    if (annotation != null)
                    {
                        Fill(annotation, schemaAttributes);
                    }
                }
            }
        }

        private void Fill(annotation source, SchemaAttributeCollection target)
        {
            if (source != null)
            {
                foreach (var item in source.Items)
                {
                    Fill(item, target);
                }
            }
        }

        private void Fill(object docOrAppInfo, SchemaAttributeCollection target)
        {
            if (docOrAppInfo != null)
            {
                var doc = docOrAppInfo as documentation;
                var app = docOrAppInfo as appInfo;

                if (doc != null)
                {
                    var attr = new MidiDeviceSchemaAttribute();

                    Fill(doc, attr);

                    target.Add(attr);
                }

                if (app != null)
                {
                    throw new NotImplementedException("The use of <annotation>/<appInfo> is not implemented yet.");
                }
            }
        }

        private void Fill(documentation source, MidiDeviceSchemaAttribute target)
        {
            target.Schema = _targetSchema;

            if (!String.IsNullOrEmpty(source.lang))
            {
                target.AttributeName = DocumentationAttributeName + "[" + source.lang + "]";
            }
            else
            {
                target.AttributeName = DocumentationAttributeName;
            }

            if (!String.IsNullOrEmpty(source.source))
            {
                target.Value = source.source;
            }
        }

        private void Fill(IEnumerable<annotated> source, DataTypeCollection dataTypes)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    var dataType = item as dataType;

                    if (dataType != null)
                    {
                        var dt = new MidiDeviceSchemaDataType();

                        Fill(dataType, dt);

                        dataTypes.Add(dt);
                    }
                }

                foreach (var pair in _deferredDataTypes)
                {
                    var baseType = dataTypes.Find(pair.Key.Name);

                    if (baseType == null)
                    {
                        throw new DeviceSchemaException("Could not find the DataType for " + pair.Key.Namespace + ":" + pair.Key.Name);
                    }

                    pair.Value.BaseTypes.Add(baseType);
                }

                _deferredDataTypes.Clear();
            }
        }

        private void Fill(dataType source, MidiDeviceSchemaDataType target)
        {
            target.Schema = _targetSchema;
            target.DataTypeName = source.name;
            target.SetIsAbstract(source.@abstract);
            target.SetBitOrder(source.bitOrder);
            target.SetValueOffset(source.valueOffset);

            Fill(source.annotation, target.Attributes);

            var extension = source.Item as extension;
            var restriction = source.Item as restriction;
            var union = source.Item as union;

            if (extension != null)
            {
                target.SetIsExtension();
                Fill(extension.baseTypes, target);

                //Fill(restriction.annotation, target.Attributes);
                Fill(extension.Items, extension.ItemsElementName, target.Constraints);
            }

            if (restriction != null)
            {
                var baseType = _schemas.FindDataType(restriction.@base.Namespace, restriction.@base.Name);

                if (baseType != null)
                {
                    target.BaseTypes.Add(baseType);
                }
                else if (restriction.@base.Namespace != XmlSchemaNamespace)
                {
                    _deferredDataTypes.Add(
                        new KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>(restriction.@base, target));
                }

                //Fill(restriction.annotation, target.Attributes);
                Fill(restriction.Items, restriction.ItemsElementName, target.Constraints);
            }

            if (union != null)
            {
                if (union.dataType != null)
                {
                    throw new DeviceSchemaException("Nested DataType inside a union is not implemented yet.");
                }

                target.SetIsUnion();
                Fill(union.memberTypes, target);

                //Fill(restriction.annotation, target.Attributes);
            }
        }

        private void Fill(IEnumerable<XmlQualifiedName> source, MidiDeviceSchemaDataType target)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    var baseType = _schemas.FindDataType(item.Namespace, item.Name);

                    if (baseType != null)
                    {
                        target.BaseTypes.Add(baseType);
                    }
                    else if (item.Namespace != XmlSchemaNamespace)
                    {
                        _deferredDataTypes.Add(
                            new KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>(item, target));
                    }
                }
            }
        }

        private void Fill(facet[] facets, ItemsChoiceType1[] choiceTypes, ConstraintCollection constraintCollection)
        {
            if (facets != null)
            {
                for (int i = 0; i < facets.Length; i++)
                {
                    var facet = facets[i];
                    var itemType = choiceTypes[i];

                    var constraint = MidiDeviceSchemaConstraint.Create(itemType.ToString(), facet.value);

                    constraintCollection.Add(constraint);
                }
            }
        }

        private void Fill(facet[] facets, ItemsChoiceType[] choiceTypes, ConstraintCollection constraintCollection)
        {
            if (facets != null)
            {
                for (int i = 0; i < facets.Length; i++)
                {
                    var facet = facets[i];
                    var itemType = choiceTypes[i];

                    var constraint = MidiDeviceSchemaConstraint.Create(itemType.ToString(), facet.value);

                    constraintCollection.Add(constraint);
                }
            }
        }

        private void Fill(IEnumerable<annotated> annotated, RecordTypeCollection recordTypeCollection)
        {
            if (annotated != null)
            {
                foreach (var item in annotated)
                {
                    var recordType = item as recordType;

                    if (recordType != null)
                    {
                        var rt = new MidiDeviceSchemaRecordType();

                        Fill(recordType, rt);

                        recordTypeCollection.Add(rt);
                        _targetSchema.RootRecordTypes.Add(rt);
                    }
                }

                foreach (var pair in _deferredRecordTypes)
                {
                    var baseType = (MidiDeviceSchemaRecordType)_targetSchema.AllRecordTypes.Find(pair.Key.Name);

                    if (baseType == null)
                    {
                        throw new DeviceSchemaException(
                            String.Format("The base type '{1}' ({0}) used for record type '{2}' was not found.",
                                pair.Key.Namespace, pair.Key.Name, pair.Value.Name));
                    }

                    pair.Value.SetBaseType(baseType);
                    _targetSchema.RootRecordTypes.Remove(baseType);
                }

                _deferredRecordTypes.Clear();

                foreach (var pair in _deferredFields)
                {
                    if (SetFieldType(pair.Key.Namespace, pair.Key.Name, pair.Value))
                    {
                        if (pair.Value.RecordType != null)
                        {
                            _targetSchema.RootRecordTypes.Remove(pair.Value.RecordType);
                        }
                    }
                    else
                    {
                        throw new DeviceSchemaException(
                            String.Format("The base type '{1}' ({0}) used for data type '{2}' was not found.",
                                pair.Key.Namespace, pair.Key.Name, pair.Value.Name));
                    }
                }

                _deferredFields.Clear();
            }
        }

        private void Fill(recordType source, MidiDeviceSchemaRecordType target)
        {
            target.Schema = _targetSchema;
            target.RecordTypeName = source.name;
            target.SetIsAbstract(source.@abstract);
            if (source.widthSpecified)
            {
                target.SetWidth((int)source.width);
            }

            Fill(source.annotation, target.Attributes);

            var restriction = source.Item as recordRestrictionType;
            var extension = source.Item as recordExtensionType;
            var sequence = source.Item as fieldSequence;

            if (restriction != null)
            {
                throw new DeviceSchemaException("RecordType restriction is not implemented yet.");
            }

            if (extension != null)
            {
                var baseType = _schemas.FindRecordType(extension.@base.Namespace, extension.@base.Name);

                if (baseType != null)
                {
                    target.SetBaseType(baseType);
                }
                else
                {
                    _deferredRecordTypes.Add(
                        new KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>(extension.@base, target));
                }

                //Fill(extension.annotation, target.Attributes);
                Fill(extension.sequence, target);
            }

            if (sequence != null)
            {
                Fill(sequence.field, target);
            }
        }

        private void Fill(fieldSequence source, MidiDeviceSchemaRecordType target)
        {
            if (source != null)
            {
                Fill(source.field, target);
            }
        }

        private void Fill(IEnumerable<localField> source, MidiDeviceSchemaRecordType recordType)
        {
            if (source != null)
            {
                foreach (var field in source)
                {
                    var fld = new MidiDeviceSchemaField();
                    fld.SetDeclaringRecord(recordType);

                    Fill(field, fld);

                    recordType.Fields.Add(fld);
                }
            }
        }

        private void Fill(localField source, MidiDeviceSchemaField target)
        {
            target.Schema = _targetSchema;
            target.SetRepeats(Int32.Parse(source.repeats));
            target.SetName(source.name);
            target.SetDevicePropertyName(source.property);
            target.SetSize(source.size);
            target.SetAddress(source.address);
            if (source.widthSpecified)
            {
                target.SetWidth((int)source.width);
            }
            SetFixedConstraint(source.@fixed, target.Constraints);

            Fill(source.annotation, target.Attributes);

            if (!SetFieldType(source.type.Namespace, source.type.Name, target))
            {
                _deferredFields.Add(
                        new KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>(source.type, target));
            }
            else
            {
                var dataType = target.DataType;

                while (dataType != null)
                {
                    target.Constraints.Merge(dataType.Constraints);

                    dataType = dataType.BaseType;
                }

                if (target.RecordType != null)
                {
                    _targetSchema.RootRecordTypes.Remove(target.RecordType);
                }
            }
        }

        private void SetFixedConstraint(string fixedValue, ConstraintCollection target)
        {
            if (!String.IsNullOrEmpty(fixedValue))
            {
                var constraint = MidiDeviceSchemaConstraint.Create("fixed", fixedValue);
                target.Add(constraint);
            }
        }

        private bool SetFieldType(string schema, string name, MidiDeviceSchemaField target)
        {
            var fullName = new SchemaObjectName(schema, name);
            var dataType = _schemas.FindDataType(schema, name);

            if (dataType == null)
            {
                dataType = (MidiDeviceSchemaDataType)_targetSchema.AllDataTypes.Find(fullName.FullName);
            }

            if (dataType == null)
            {
                var recordType = _schemas.FindRecordType(schema, name);

                if (recordType == null)
                {
                    recordType = (MidiDeviceSchemaRecordType)_targetSchema.AllRecordTypes.Find(fullName.FullName);

                    if (recordType == null)
                    {
                        return false;
                    }
                }

                target.SetRecordType(recordType);
            }
            else
            {
                target.SetDataType(dataType);
            }

            return true;
        }
    }
}