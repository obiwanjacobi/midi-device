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

    private readonly DeviceSchemaSet _schemas;

    public MidiDeviceSchemaParser(DeviceSchemaSet schemas)
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

        ProcessImports(sourceSchema.Items);
        CreateTargetSchema(sourceSchema);
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

            var loader = new SchemaLoader(_schemas);
            _ = loader.LoadSchema(
                SchemaName.FromAssemblyResource(import.assembly, import.name));
        }
    }

    private DeviceSchema _targetSchema;
    private readonly string DocumentationAttributeName = "Documentation";
    private readonly List<KeyValuePair<XmlQualifiedName, DataType>> _deferredDataTypes = new();
    private readonly List<KeyValuePair<XmlQualifiedName, RecordType>> _deferredRecordTypes = new();
    private readonly List<KeyValuePair<XmlQualifiedName, Field>> _deferredFields = new();

    protected virtual DeviceSchema CreateTargetSchema(deviceSchema source)
    {
        _targetSchema = new DeviceSchema(source.schema)
        {
            Version = source.version
        };
        
        _schemas.Add(_targetSchema);

        FillAttributes(source.Items, _targetSchema.Attributes);
        FillDataTypes(source.Items1, _targetSchema.AllDataTypes);
        FillRecordTypes(source.Items1, _targetSchema.AllRecordTypes);
        FillVirtulaRootFields(_targetSchema);

        return _targetSchema;
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
                var attr = CreateDocumentationAttribute(doc);
                target.Add(attr);
            }

            if (docOrAppInfo is appInfo)
            {
                throw new NotImplementedException(
                    "The use of <annotation>/<appInfo> is not implemented yet.");
            }
        }
    }

    private SchemaAttribute CreateDocumentationAttribute(documentation source)
    {
        string name;
        if (!String.IsNullOrEmpty(source.lang))
        {
            name = $"{DocumentationAttributeName} [{source.lang}]";
        }
        else
        {
            name = DocumentationAttributeName;
        }

        var value = String.Empty;
        if (source.Any is not null)
        {
            var texts = from node in source.Any
                        where node is XmlText
                        select ((XmlText)node).Value;

            value = String.Join("\r\n", texts);
        }

        var target = new SchemaAttribute(_targetSchema, CreateName(name), value);
        return target;
    }

    private SchemaObjectName CreateName(string name)
        => new(_targetSchema.SchemaName, name);

    private void FillDataTypes(IEnumerable<annotated> source, DataTypeCollection dataTypes)
    {
        if (source != null)
        {
            foreach (annotated item in source)
            {
                var dataType = item as dataType;

                if (dataType != null)
                {
                    var dt = CreateDataType(dataType);
                    dataTypes.Add(dt);
                }
            }

            foreach (KeyValuePair<XmlQualifiedName, DataType> pair in _deferredDataTypes)
            {
                DataType baseType = dataTypes.Find(pair.Key.Name)
                    ?? throw new DeviceSchemaException(
                        $"Could not find the DataType for {pair.Key.Namespace}:{pair.Key.Name}");

                pair.Value.BaseTypes.Add(baseType);
            }

            _deferredDataTypes.Clear();
        }
    }

    private DataType CreateDataType(dataType source)
    {
        var target = new DataType(_targetSchema, CreateName(source.name))
        {
            IsAbstract = source.@abstract,
            ValueOffset = source.valueOffset,
            BitOrder = source.bitOrder == bitOrder.LittleEndian
                ? BitOrder.LittleEndian : BitOrder.BigEndian
        };

        if (!String.IsNullOrWhiteSpace(source.range))
            target.Range = new ValueRange(source.range);

        FillAttributed(source.annotation, target.Attributes);

        if (source.Item is extension extension)
        {
            target.IsExtension = true;
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
                    new KeyValuePair<XmlQualifiedName, DataType>(restriction.@base, target));
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

            target.IsUnion = true;
            FillNames(union.memberTypes, target);
            FillAttributed(union.annotation, target.Attributes);
        }

        return target;
    }

    private void FillNames(IEnumerable<XmlQualifiedName> source, DataType target)
    {
        if (source != null)
        {
            foreach (XmlQualifiedName item in source)
            {
                DataType baseType = _schemas.FindDataType(item.Namespace, item.Name);

                if (baseType != null)
                {
                    target.BaseTypes.Add(baseType);
                }
                else if (item.Namespace != XmlSchemaNamespace)
                {
                    _deferredDataTypes.Add(
                        new KeyValuePair<XmlQualifiedName, DataType>(item, target));
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

                var constraint = Constraint.Create(itemType.ToString(), facet.value);
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

                var constraint = Constraint.Create(itemType.ToString(), facet.value);

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
                    var rt = CreateRecordType(recordType);

                    recordTypeCollection.Add(rt);
                    _targetSchema.RootRecordTypes.Add(rt);
                }
            }

            foreach (KeyValuePair<XmlQualifiedName, RecordType> pair in _deferredRecordTypes)
            {
                var baseType = _targetSchema.AllRecordTypes.Find(pair.Key.Name)
                    ?? throw new DeviceSchemaException(
                        $"The base type '{pair.Key.Name}' ({pair.Key.Namespace}) used for record type '{pair.Value.Name}' was not found.");

                pair.Value.BaseType = baseType;
                _targetSchema.RootRecordTypes.Remove(baseType);
            }

            _deferredRecordTypes.Clear();

            foreach (KeyValuePair<XmlQualifiedName, Field> pair in _deferredFields)
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

    private RecordType CreateRecordType(recordType source)
    {
        var target = new RecordType(_targetSchema, CreateName(source.name))
        {
            IsAbstract = source.@abstract
        };

        if (source.widthSpecified)
        {
            target.Width = (int)source.width;
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
                target.BaseType = baseType;
            }
            else
            {
                _deferredRecordTypes.Add(
                    new KeyValuePair<XmlQualifiedName, RecordType>(extension.@base, target));
            }

            FillFields(extension.sequence, target);
        }

        if (source.Item is fieldSequence sequence)
        {
            FillFields(sequence.field, target);
        }

        return target;
    }

    private void FillFields(fieldSequence source, RecordType target)
    {
        if (source != null)
        {
            FillFields(source.field, target);
        }
    }

    private void FillFields(IEnumerable<localField> source, RecordType recordType)
    {
        if (source != null)
        {
            foreach (var field in source)
            {
                var fld = CreateField(field);

                fld.DeclaringRecord = recordType;
                recordType.Fields.Add(fld);
            }
        }
    }

    private Field CreateField(localField source)
    {
        var target = new Field(_targetSchema, CreateName(source.name));

        target.Properties.Repeats = Int32.Parse(source.repeats);
        target.Properties.DevicePropertyName = source.property;
        target.Properties.Size = new Core.SevenBitUInt32(source.size);
        target.Properties.Address = new Core.SevenBitUInt32(source.address);
        if (!String.IsNullOrWhiteSpace(source.range))
            target.Properties.Range = new Core.ValueRange(source.range);

        if (source.widthSpecified)
        {
            target.Properties.Width = (int)source.width;
        }

        SetFixedConstraint(source.@fixed, target.Constraints);

        FillAttributed(source.annotation, target.Attributes);

        if (!SetFieldType(source.type.Namespace, source.type.Name, target))
        {
            _deferredFields.Add(
                    new KeyValuePair<XmlQualifiedName, Field>(source.type, target));
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

        return target;
    }

    private static void SetFixedConstraint(string fixedValue, ConstraintCollection target)
    {
        if (!String.IsNullOrEmpty(fixedValue))
        {
            var constraint = Constraint.Create("fixed", fixedValue);
            target.Add(constraint);
        }
    }

    private bool SetFieldType(string schema, string name, Field target)
    {
        SchemaObjectName fullName = new(schema, name);
        var dataType = _schemas.FindDataType(schema, name)
            ?? _targetSchema.AllDataTypes.Find(fullName.FullName);

        if (dataType == null)
        {
            var recordType = _schemas.FindRecordType(schema, name);

            if (recordType == null)
            {
                recordType = _targetSchema.AllRecordTypes.Find(fullName.FullName);

                if (recordType == null)
                {
                    return false;
                }
            }

            target.RecordType = recordType;
        }
        else
        {
            target.DataType = dataType;
        }

        return true;
    }

    private static void FillVirtulaRootFields(DeviceSchema target)
    {
        foreach (RecordType root in target.RootRecordTypes)
        {
            var field = new Field(target, root.Name.FullName)
            {
                RecordType = root
            };

            target.VirtualRootFields.Add(field);
        }
    }
}