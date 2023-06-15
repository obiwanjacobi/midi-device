using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CannedBytes.Midi.Core;
using CannedBytes.Midi.Device.Schema.Xml.Model1;

namespace CannedBytes.Midi.Device.Schema.Xml;

public class MidiDeviceSchemaParser
{
    private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

    private readonly MidiDeviceSchemaSet _schemas;

    public MidiDeviceSchemaParser(MidiDeviceSchemaSet schemas)
    {
        Assert.IfArgumentNull(schemas, nameof(schemas));

        _schemas = schemas;
    }

    public DeviceSchema Parse(Stream stream)
    {
        Assert.IfArgumentNull(stream, nameof(stream));

        var sourceSchema = MidiDeviceSchemaReader.Read(stream)
            ?? throw new DeviceSchemaException(
                "The provided stream could not be parsed into a Midi Device Schema.");
        
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

        foreach (object item in importsAndAnnotations)
        {
            var import = item as import;

            if (import != null)
            {
                ProcessImport(import);
            }
        }
    }

    private void ProcessImport(import import)
    {
        var deviceSchema = _schemas.Find(import.schema);

        if (deviceSchema == null)
        {
            Tracer.TraceEvent(
                System.Diagnostics.TraceEventType.Information,
                "Parser: Importing '{0}' from assembly '{1}'.", import.name, import.assembly);

            _ = MidiDeviceSchemaImportResolver.LoadSchema(
                _schemas, import.name, import.assembly);
        }
    }

    private MidiDeviceSchema _targetSchema;
    private readonly string DocumentationAttributeName = "Documentation";
    private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType>> _deferredDataTypes = new();
    private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType>> _deferredRecordTypes = new();
    private readonly List<KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField>> _deferredFields = new();

    protected virtual void FillSchema(deviceSchema source, MidiDeviceSchema target)
    {
        Assert.IfArgumentNull(source, nameof(source));
        Assert.IfArgumentNull(target, nameof(target));

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
            foreach (openAttrs openAttr in source)
            {
                if (openAttr is annotation annotation)
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
            foreach (object item in source.Items)
            {
                FillAppInfo(item, target);
            }
        }
    }

    private void FillAppInfo(object docOrAppInfo, SchemaAttributeCollection target)
    {
        if (docOrAppInfo != null)
        {
            if (docOrAppInfo is documentation doc)
            {
                MidiDeviceSchemaAttribute attr = new();

                FillDocumentation(doc, attr);

                target.Add(attr);
            }

            if (docOrAppInfo is appInfo)
            {
                throw new NotImplementedException(
                    "The use of <annotation>/<appInfo> is not implemented yet.");
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
            foreach (annotated item in source)
            {
                var dataType = item as dataType;

                if (dataType != null)
                {
                    var dt = new MidiDeviceSchemaDataType();

                    FillDataType(dataType, dt);

                    dataTypes.Add(dt);
                }
            }

            foreach (KeyValuePair<XmlQualifiedName, MidiDeviceSchemaDataType> pair in _deferredDataTypes)
            {
                DataType baseType = dataTypes.Find(pair.Key.Name)
                    ?? throw new DeviceSchemaException(
                        $"Could not find the DataType for {pair.Key.Namespace}:{pair.Key.Name}");

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

        if (source.Item is extension extension)
        {
            target.SetIsExtension();
            FillNames(extension.baseTypes, target);

            FillAttributed(extension.annotation, target.Attributes);
            FillFacets(extension.Items, extension.ItemsElementName, target.Constraints);
        }

        if (source.Item is restriction restriction)
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

        if (source.Item is union union)
        {
            if (union.dataType != null)
            {
                throw new DeviceSchemaException(
                    "Nested DataType inside a union is not implemented yet.");
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
            foreach (XmlQualifiedName item in source)
            {
                MidiDeviceSchemaDataType baseType = _schemas.FindDataType(item.Namespace, item.Name);

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

    private static void FillFacets(facet[] facets, ItemsChoiceType1[] choiceTypes, ConstraintCollection constraintCollection)
    {
        if (facets != null)
        {
            for (int i = 0; i < facets.Length; i++)
            {
                facet facet = facets[i];
                ItemsChoiceType1 itemType = choiceTypes[i];

                MidiDeviceSchemaConstraint constraint = MidiDeviceSchemaConstraint.Create(itemType.ToString(), facet.value);

                constraintCollection.Add(constraint);
            }
        }
    }

    private static void FillFacets(facet[] facets, ItemsChoiceType[] choiceTypes, ConstraintCollection constraintCollection)
    {
        if (facets != null)
        {
            for (int i = 0; i < facets.Length; i++)
            {
                facet facet = facets[i];
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
            foreach (annotated item in annotated)
            {
                if (item is recordType recordType)
                {
                    MidiDeviceSchemaRecordType rt = new();

                    FillRecordType(recordType, rt);

                    recordTypeCollection.Add(rt);
                    _targetSchema.RootRecordTypes.Add(rt);
                }
            }

            foreach (KeyValuePair<XmlQualifiedName, MidiDeviceSchemaRecordType> pair in _deferredRecordTypes)
            {
                var baseType = (MidiDeviceSchemaRecordType)_targetSchema.AllRecordTypes.Find(pair.Key.Name)
                    ?? throw new DeviceSchemaException(
                        $"The base type '{pair.Key.Name}' ({pair.Key.Namespace}) used for record type '{pair.Value.Name}' was not found.");

                pair.Value.SetBaseType(baseType);
                _targetSchema.RootRecordTypes.Remove(baseType);
            }

            _deferredRecordTypes.Clear();

            foreach (KeyValuePair<XmlQualifiedName, MidiDeviceSchemaField> pair in _deferredFields)
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
                        $"The base type '{pair.Key.Name}' ({pair.Key.Namespace}) used for data type '{pair.Value.Name}' was not found.");
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

        if (source.Item is recordRestrictionType)
        {
            throw new DeviceSchemaException(
                "RecordType restriction is not implemented yet.");
        }

        if (source.Item is recordExtensionType extension)
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

        if (source.Item is fieldSequence sequence)
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
                MidiDeviceSchemaField fld = new();
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

    private static void SetFixedConstraint(string fixedValue, ConstraintCollection target)
    {
        if (!String.IsNullOrEmpty(fixedValue))
        {
            var constraint = MidiDeviceSchemaConstraint.Create("fixed", fixedValue);
            target.Add(constraint);
        }
    }

    private bool SetFieldType(string schema, string name, MidiDeviceSchemaField target)
    {
        SchemaObjectName fullName = new(schema, name);
        var dataType = _schemas.FindDataType(schema, name)
            ?? (MidiDeviceSchemaDataType)_targetSchema.AllDataTypes.Find(fullName.FullName);

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
            MidiDeviceSchemaField field = new();

            field.SetRecordType(root);
            field.SetName(root.Name.FullName);
            field.Schema = target;

            target.VirtualRootFields.Add(field);
        }
    }
}