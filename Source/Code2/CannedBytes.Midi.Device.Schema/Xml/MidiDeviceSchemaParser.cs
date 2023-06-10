using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using CannedBytes.Midi.Device.Schema.Xml.Model1;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public class MidiDeviceSchemaParser
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        private readonly MidiDeviceSchemaSet _schemas;

        public MidiDeviceSchemaParser(MidiDeviceSchemaSet schemas)
        {
            Check.IfArgumentNull(schemas, "schemas");

            this._schemas = schemas;
        }

        public DeviceSchema Parse(Stream stream)
        {
            Check.IfArgumentNull(stream, "stream");

            var sourceSchema = MidiDeviceSchemaReader.Read(stream);

            if (sourceSchema == null)
            {
                throw new DeviceSchemaException(
                    "The provided stream could not be parsed into a Midi Device Schema.");
            }

            _targetSchema = new MidiDeviceSchema();

            ProcessImports(sourceSchema.Items);

            FillSchema(sourceSchema, _targetSchema);

            _schemas.Add(_targetSchema);

            return _targetSchema;
        }

        private void ProcessImports(object[] importsAndAnnotations)
        {
            if (importsAndAnnotations == null) return;

            Tracer.TraceEvent(
                System.Diagnostics.TraceEventType.Information,
                "Parser: Importing {0} external schemas.", 
                (from ia in importsAndAnnotations where ia is import select ia).Count());

            foreach (var item in importsAndAnnotations)
            {
                var import = item as import;

                if (import != null)
                {
                    ProcessImport(import);
                }
            }
        }

        private DeviceSchema ProcessImport(import import)
        {
            var deviceSchema = _schemas.Find(import.schema);

            if (deviceSchema == null)
            {
                Tracer.TraceEvent(
                    System.Diagnostics.TraceEventType.Information,
                    "Parser: Importing '{0}' from assembly '{1}'.", import.name, import.assembly);

                deviceSchema = MidiDeviceSchemaImportResolver.LoadSchema(
                    _schemas, import.name, import.assembly);
            }

            return deviceSchema;
        }

        

        private MidiDeviceSchema _targetSchema;
        private readonly string DocumentationAttributeName = "Documentation";
        private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>> _deferredDataTypes = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>>();
        private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>> _deferredRecordTypes = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>>();
        private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>> _deferredFields = new List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>>();

        protected virtual void FillSchema(deviceSchema source, MidiDeviceSchema target)
        {
            Check.IfArgumentNull(source, "source");
            Check.IfArgumentNull(target, "target");

            target.SchemaName = source.schema;
            target.SetVersion(source.version);

            FillAttributes(source.Items, target.Attributes);
            FillDataTypes(source.Items1, target.AllDataTypes);
            FillRecordTypes(source.Items1, target.AllRecordTypes);
            FillVirtulaRootFields(target);
        }

        private void FillAttributes(IEnumerable<openAttrs> source, SchemaAttributeCollection schemaAttributes)
        {
            if (source != null)
            {
                foreach (var openAttr in source)
                {
                    var annotation = openAttr as annotation;

                    if (annotation != null)
                    {
                        FillAttributed(annotation, schemaAttributes);
                    }
                }
            }
        }

        private void FillAttributed(annotation source, SchemaAttributeCollection target)
        {
            if (source != null)
            {
                foreach (var item in source.Items)
                {
                    FillAppInfo(item, target);
                }
            }
        }

        private void FillAppInfo(object docOrAppInfo, SchemaAttributeCollection target)
        {
            if (docOrAppInfo != null)
            {
                var doc = docOrAppInfo as documentation;
                var app = docOrAppInfo as appInfo;

                if (doc != null)
                {
                    var attr = new MidiDeviceSchemaAttribute();

                    FillDocumentation(doc, attr);

                    target.Add(attr);
                }

                if (app != null)
                {
                    throw new NotImplementedException("The use of <annotation>/<appInfo> is not implemented yet.");
                }
            }
        }

        private void FillDocumentation(documentation source, MidiDeviceSchemaAttribute target)
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

            if (source.Any != null)
            {
                var texts = from node in source.Any
                            where node is XmlText
                            select ((XmlText)node).Value;

                var value = String.Join("\r\n", texts);

                target.SetValue(value);
            }
        }

        private void FillDataTypes(IEnumerable<annotated> source, DataTypeCollection dataTypes)
        {
            if (source != null)
            {
                foreach (var item in source)
                {
                    var dataType = item as dataType;

                    if (dataType != null)
                    {
                        var dt = new MidiDeviceSchemaDataType();

                        FillDataType(dataType, dt);

                        dataTypes.Add(dt);
                    }
                }

                foreach (var pair in _deferredDataTypes)
                {
                    var baseType = dataTypes.Find(pair.Key.Name)
                        ?? throw new DeviceSchemaException("Could not find the DataType for " + pair.Key.Namespace + ":" + pair.Key.Name);
                    pair.Value.BaseTypes.Add(baseType);
                }

                _deferredDataTypes.Clear();
            }
        }

        private void FillDataType(dataType source, MidiDeviceSchemaDataType target)
        {
            target.Schema = _targetSchema;
            target.DataTypeName = source.name;
            target.SetIsAbstract(source.@abstract);
            target.SetBitOrder(source.bitOrder);
            target.SetValueOffset(source.valueOffset);

            FillAttributed(source.annotation, target.Attributes);

            var extension = source.Item as extension;
            var restriction = source.Item as restriction;
            var union = source.Item as union;

            if (extension != null)
            {
                target.SetIsExtension();
                FillNames(extension.baseTypes, target);

                FillAttributed(extension.annotation, target.Attributes);
                FillFacets(extension.Items, extension.ItemsElementName, target.Constraints);
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

                FillAttributed(restriction.annotation, target.Attributes);
                FillFacets(restriction.Items, restriction.ItemsElementName, target.Constraints);
            }

            if (union != null)
            {
                if (union.dataType != null)
                {
                    throw new DeviceSchemaException("Nested DataType inside a union is not implemented yet.");
                }

                target.SetIsUnion();
                FillNames(union.memberTypes, target);
                FillAttributed(union.annotation, target.Attributes);
            }
        }

        private void FillNames(IEnumerable<XmlQualifiedName> source, MidiDeviceSchemaDataType target)
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

        private void FillFacets(facet[] facets, ItemsChoiceType1[] choiceTypes, ConstraintCollection constraintCollection)
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

        private void FillFacets(facet[] facets, ItemsChoiceType[] choiceTypes, ConstraintCollection constraintCollection)
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

        private void FillRecordTypes(IEnumerable<annotated> annotated, RecordTypeCollection recordTypeCollection)
        {
            if (annotated != null)
            {
                foreach (var item in annotated)
                {
                    var recordType = item as recordType;

                    if (recordType != null)
                    {
                        var rt = new MidiDeviceSchemaRecordType();

                        FillRecordType(recordType, rt);

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

        private void FillRecordType(recordType source, MidiDeviceSchemaRecordType target)
        {
            target.Schema = _targetSchema;
            target.RecordTypeName = source.name;
            target.SetIsAbstract(source.@abstract);
            if (source.widthSpecified)
            {
                target.SetWidth((int)source.width);
            }

            FillAttributed(source.annotation, target.Attributes);

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

                FillFields(extension.sequence, target);
            }

            if (sequence != null)
            {
                FillFields(sequence.field, target);
            }
        }

        private void FillFields(fieldSequence source, MidiDeviceSchemaRecordType target)
        {
            if (source != null)
            {
                FillFields(source.field, target);
            }
        }

        private void FillFields(IEnumerable<localField> source, MidiDeviceSchemaRecordType recordType)
        {
            if (source != null)
            {
                foreach (var field in source)
                {
                    var fld = new MidiDeviceSchemaField();
                    fld.SetDeclaringRecord(recordType);

                    FillField(field, fld);

                    recordType.Fields.Add(fld);
                }
            }
        }

        private void FillField(localField source, MidiDeviceSchemaField target)
        {
            target.Schema = _targetSchema;
            target.SetName(source.name);

            target.ExtendedProperties.Repeats = Int32.Parse(source.repeats);
            target.ExtendedProperties.DevicePropertyName = source.property;
            target.ExtendedProperties.Size = new Core.SevenBitUInt32(source.size);
            target.ExtendedProperties.Address = new Core.SevenBitUInt32(source.address);
            if (source.widthSpecified)
            {
                target.ExtendedProperties.Width = (int)source.width;
            }

            SetFixedConstraint(source.@fixed, target.Constraints);

            FillAttributed(source.annotation, target.Attributes);

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

        private static void FillVirtulaRootFields(MidiDeviceSchema target)
        {
            foreach (MidiDeviceSchemaRecordType root in target.RootRecordTypes)
            {
                var field = new MidiDeviceSchemaField();

                field.SetRecordType(root);
                field.SetName(root.Name.FullName);
                field.Schema = target;
                
                target.VirtualRootFields.Add(field);
            }
        }
    }
}