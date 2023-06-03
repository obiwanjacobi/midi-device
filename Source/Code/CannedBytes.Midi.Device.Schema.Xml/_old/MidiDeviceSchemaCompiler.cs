using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using CannedBytes.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    internal class MidiDeviceSchemaCompiler
    {
        private const string TraceCategory = "Midi Device Schema Compiler";
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        public MidiDeviceSchemaCompiler(MidiDeviceSchemaManager schemaMgr)
        {
            Check.IfArgumentNull(schemaMgr, "schemaMgr");

            _schemaMgr = schemaMgr;
        }

        private MidiDeviceSchemaManager _schemaMgr;

        public MidiDeviceSchemaManager SchemaManager
        {
            get { return _schemaMgr; }
        }

        public void Compile(MidiDeviceSchema schema)
        {
            Check.IfArgumentNull(schema, "schema");

            XmlSchemaNavigator navigator = new XmlSchemaNavigator(schema.XmlSchema);

            InitializeTypes(schema, navigator);

            BuildRecordTypeHierarchy(schema, navigator);

            BuildDataTypeHierarchy(schema, navigator);

            InitializeFields(schema, navigator);

            BuildFlattendFieldList(schema);

            //InitializeSearchList(schema);
        }

        #region Midi Device Schema Building methods

        private void InitializeTypes(MidiDeviceSchema schema, XmlSchemaNavigator navigator)
        {
            XmlSchemaNavigationContext context = new XmlSchemaNavigationContext();

            context.NavigateImport += delegate(object importSource, XmlSchemaNavigatorEventArgs<XmlSchemaImport> importEventArgs)
                {
                    Debug.Assert(SchemaManager.Xml.Schemas.Contains(importEventArgs.XmlObject.Schema));

                    // creates the schema if it does not exist.
                    MidiDeviceSchema importSchema = SchemaManager.Open(importEventArgs.XmlObject.Namespace);
                };

            context.NavigateComplexType += delegate(object typeEventSrc, XmlSchemaNavigatorEventArgs<XmlSchemaComplexType> typeEventArgs)
                {
                    MidiDeviceSchemaRecordType schemaType = GetDeviceSchemaType(schema, typeEventArgs.XmlObject, true);

                    AddAttributes(schemaType, typeEventArgs.XmlObject);
                };

            context.NavigateSimpleType += delegate(object simpleTypeEventSource, XmlSchemaNavigatorEventArgs<XmlSchemaSimpleType> simpleTypeEventArgs)
                {
                    MidiDeviceSchemaDataType dataType = GetDeviceDataType(schema, simpleTypeEventArgs.XmlObject, true);

                    AddAttributes(dataType, simpleTypeEventArgs.XmlObject);
                };

            navigator.NavigateSchema(context);
        }

        private void BuildRecordTypeHierarchy(MidiDeviceSchema schema, XmlSchemaNavigator navigator)
        {
            // first copy all types into the root types list
            // when building the hierarchy all base-types that are removed from the root types list
            schema.RootRecordTypes.AddRange(schema.AllRecordTypes);

            XmlSchemaNavigationContext context = new XmlSchemaNavigationContext();
            MidiDeviceSchemaRecordType schemaType = null;

            // complex extensions indicate a derived schema type
            context.NavigateComplexContentExtension += delegate(object typeEventSource,
                XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentExtension> typeEventArgs)
                {
                    MidiDeviceSchemaRecordType baseType = (MidiDeviceSchemaRecordType)
                        FindRecordType(schema, typeEventArgs.XmlObject.BaseTypeName);

                    schemaType.BaseType = baseType;

                    // baseType is not a root type
                    schema.RootRecordTypes.Remove(baseType);
                };

            for (int n = 0; n < schema.AllRecordTypes.Count; n++)
            {
                schemaType = (MidiDeviceSchemaRecordType)schema.AllRecordTypes[n];
                navigator.NavigateBaseType(context, schemaType.XmlType);
            }
        }

        private void BuildDataTypeHierarchy(MidiDeviceSchema schema, XmlSchemaNavigator navigator)
        {
            XmlSchemaNavigationContext context = new XmlSchemaNavigationContext();
            MidiDeviceSchemaDataType dataType = null;

            // simple type restrictions indicate a derived data type
            context.NavigateSimpleTypeRestriction += delegate(object typeEventSource,
                XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeRestriction> typeEventArgs)
                {
                    InitializeConstraints(dataType, typeEventArgs.XmlObject.Facets);

                    MidiDeviceSchemaDataType baseType = null;

                    if (typeEventArgs.XmlObject.BaseTypeName != null && !typeEventArgs.XmlObject.BaseTypeName.IsEmpty)
                    {
                        baseType = FindDataType(schema, typeEventArgs.XmlObject.BaseTypeName);
                    }

                    if (baseType == null && typeEventArgs.XmlObject.BaseType != null)
                    {
                        baseType = GetDeviceDataType(schema, typeEventArgs.XmlObject.BaseType, true);
                    }

                    if (baseType != null)
                    {
                        dataType.BaseTypes.Add(baseType);
                    }
                    else if (typeEventArgs.XmlObject.BaseTypeName.Namespace != XmlSchemaNamespace)
                    {
                        Debug.WriteLine(String.Format("Could not find base type {0} for Data Type {1}",
                            typeEventArgs.XmlObject.BaseTypeName, dataType.Name.FullName), TraceCategory);
                    }
                };

            context.NavigateSimpleTypeUnion += delegate(object unionEventSource,
                XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeUnion> unionEventArgs)
                {
                    foreach (XmlSchemaSimpleType unionType in unionEventArgs.XmlObject.BaseMemberTypes)
                    {
                        MidiDeviceSchemaDataType baseType = SchemaManager.FindDataType(unionType.QualifiedName);

                        if (baseType == null)
                        {
                            baseType = GetDeviceDataType(schema, unionType, true);
                        }

                        if (baseType != null)
                        {
                            dataType.BaseTypes.Add(baseType);
                        }
                    }
                };

            for (int n = 0; n < schema.AllDataTypes.Count; n++)
            {
                dataType = (MidiDeviceSchemaDataType)schema.AllDataTypes[n];
                navigator.NavigateBaseType(context, dataType.XmlType);
            }
        }

        private void InitializeFields(MidiDeviceSchema schema, XmlSchemaNavigator navigator)
        {
            XmlSchemaNavigationContext context = new XmlSchemaNavigationContext();
            MidiDeviceSchemaRecordType schemaType = null;

            context.NavigateElement += delegate(object elementEventSrc,
                XmlSchemaNavigatorEventArgs<XmlSchemaElement> elementEventArgs)
                {
                    MidiDeviceSchemaField field = new MidiDeviceSchemaField(elementEventArgs.XmlObject);
                    field.DeclaringRecord = schemaType;

                    // assign minOccors/maxOccurs values when set.
                    if (!String.IsNullOrEmpty(elementEventArgs.XmlObject.MaxOccursString))
                    {
                        field.Repeats = Int32.Parse(elementEventArgs.XmlObject.MaxOccursString);
                    }
                    //if (!String.IsNullOrEmpty(elementEventArgs.XmlObject.MinOccursString))
                    //{
                    //    field.MinOccurs = Int32.Parse(elementEventArgs.XmlObject.MinOccursString);
                    //}

                    Debug.Assert(elementEventArgs.XmlObject.ElementSchemaType != null, "No type for element found.");

                    XmlSchemaSimpleType simpleType =
                        elementEventArgs.XmlObject.ElementSchemaType as XmlSchemaSimpleType;

                    if (simpleType != null)
                    {
                        field.DataType = FindDataType(schema, simpleType.QualifiedName);

                        Debug.Assert(field.DataType != null,
                            String.Format("DataType {0} could not be found for Field {1}.", simpleType.QualifiedName, field.Name.FullName));
                    }

                    XmlSchemaComplexType complexType =
                        elementEventArgs.XmlObject.ElementSchemaType as XmlSchemaComplexType;

                    if (complexType != null)
                    {
                        MidiDeviceSchemaRecordType fieldType = GetDeviceSchemaType(schema, complexType, false);
                        Debug.Assert(fieldType != null);

                        field.RecordType = fieldType;

                        // fieldType is not a root type
                        schema.RootRecordTypes.Remove(fieldType);
                    }

                    schemaType.Fields.Add(field);

                    // populate attributes after the field has been added to the collection
                    // to promote the Schema property value thru the object hierarchy.
                    AddAttributes(field, elementEventArgs.XmlObject);

                    InitializeConstraints(field);
                };

            for (int n = 0; n < schema.AllRecordTypes.Count; n++)
            {
                schemaType = (MidiDeviceSchemaRecordType)schema.AllRecordTypes[n];
                navigator.NavigateElements(context, schemaType.XmlType);
            }
        }

        private void AddAttributes(AttributedSchemaObject schemaObject, XmlSchemaAnnotated annotated)
        {
            if (schemaObject != null && annotated != null)
            {
                // unhandled attributes that were specified in the schema
                if (annotated.UnhandledAttributes != null)
                {
                    foreach (XmlAttribute attr in annotated.UnhandledAttributes)
                    {
                        SchemaAttribute schemaAttribute = new MidiDeviceSchemaAttribute(attr);

                        schemaObject.Attributes.Add(schemaAttribute);
                    }
                }

                // documentation that was specified in the schema
                if (annotated.Annotation != null)
                {
                    foreach (XmlSchemaObject annotationItem in annotated.Annotation.Items)
                    {
                        XmlSchemaDocumentation docs = annotationItem as XmlSchemaDocumentation;

                        if (docs != null)
                        {
                            schemaObject.Attributes.Add(new MidiDeviceSchemaDocumentationAttribute(schemaObject, docs));
                        }
                    }
                }

                // source location in the file of the schemaObject
                schemaObject.Attributes.Add(
                    new SchemaAttribute(schemaObject.Schema,
                        new SchemaObjectName(schemaObject.Name.FullName, "SourceLocation"),
                        XmlSchemaManager.FormatSourceLocation(annotated)));
            }
        }

        private MidiDeviceSchemaDataType FindDataType(MidiDeviceSchema schema, XmlQualifiedName name)
        {
            MidiDeviceSchemaDataType dataType = null;

            // is it a type that belongs to the schema being compiled?
            if (schema.Name == name.Namespace)
            {
                dataType = (MidiDeviceSchemaDataType)schema.AllDataTypes.Find(name.Name);
            }
            else if (name.Namespace != XmlSchemaNamespace)
            {
                // is it an imported type?
                dataType = SchemaManager.FindDataType(name);

                if (dataType == null)
                {
                    Debug.WriteLine(String.Format("Imported type {0} was not found!", name), TraceCategory);
                }
            }

            return dataType;
        }

        private MidiDeviceSchemaDataType GetDeviceDataType(MidiDeviceSchema schema, XmlSchemaSimpleType xmlType, bool createIfNotExist)
        {
            MidiDeviceSchemaDataType dataType = FindDataType(schema, xmlType.QualifiedName);

            if (dataType == null && createIfNotExist)
            {
                dataType = new MidiDeviceSchemaDataType(xmlType);

                schema.AllDataTypes.Add(dataType);
            }

            return dataType;
        }

        private MidiDeviceSchemaRecordType FindRecordType(MidiDeviceSchema schema, XmlQualifiedName name)
        {
            MidiDeviceSchemaRecordType schemaType = null;

            // is it a type that belongs to the schema being compiled?
            if (schema.Name == name.Namespace)
            {
                schemaType = (MidiDeviceSchemaRecordType)schema.AllRecordTypes.Find(name.Name);
            }
            else if (name.Namespace != XmlSchemaNamespace)
            {
                // is it an imported type?
                schemaType = SchemaManager.FindRecordType(name);

                if (schemaType == null)
                {
                    Debug.WriteLine(String.Format("Imported type {0} was not found!", name), TraceCategory);
                }
            }

            return schemaType;
        }

        private MidiDeviceSchemaRecordType GetDeviceSchemaType(MidiDeviceSchema schema, XmlSchemaComplexType xmlType, bool createIfNotExist)
        {
            MidiDeviceSchemaRecordType schemaType = FindRecordType(schema, xmlType.QualifiedName);

            if (schemaType == null && createIfNotExist)
            {
                schemaType = new MidiDeviceSchemaRecordType(xmlType);

                schema.AllRecordTypes.Add(schemaType);
            }

            return schemaType;
        }

        private void InitializeConstraints(MidiDeviceSchemaDataType dataType, XmlSchemaObjectCollection factes)
        {
            if (factes != null && factes.Count > 0)
            {
                foreach (XmlSchemaFacet facet in factes)
                {
                    MidiDeviceSchemaConstraint contraint =
                        MidiDeviceSchemaConstraint.Create(facet);

                    dataType.Constraints.Add(contraint);
                }
            }
        }

        private void InitializeConstraints(MidiDeviceSchemaField field)
        {
            if (field.DataType != null)
            {
                DataTypeCollection typeList = new DataTypeCollection();
                BuildBaseTypeList(typeList, field.DataType);

                foreach (MidiDeviceSchemaDataType dataType in typeList)
                {
                    foreach (MidiDeviceSchemaConstraint constraint in dataType.Constraints)
                    {
                        field.Constraints.Add(constraint);
                    }
                }
            }
        }

        #endregion Midi Device Schema Building methods

        #region Flattend Field List methods

        private void BuildFlattendFieldList(MidiDeviceSchema schema)
        {
            foreach (MidiDeviceSchemaRecordType rootType in schema.RootRecordTypes)
            {
                BuildFlattenedFieldList(rootType.FlattenedFields, rootType);
            }
        }

        private void BuildFlattenedFieldList(FieldCollection fields, RecordType recordType)
        {
            RecordTypeCollection baseTypes = new RecordTypeCollection();
            BuildBaseTypeList(baseTypes, recordType);

            for (int n = 0; n < baseTypes.Count; n++)
            {
                RecordType baseType = baseTypes[n];

                foreach (Field field in baseType.Fields)
                {
                    if (field.RecordType != null)
                    {
                        // from most derived (super) type to base type
                        for (int i = baseTypes.Count - 1; i >= n; i--)
                        {
                            RecordType subType = baseTypes[i];
                            Field overrideField = subType.Fields.Find(field.Name.Name);

                            if (overrideField != null)
                            {
                                Debug.Assert(overrideField.RecordType != null);

                                BuildFlattenedFieldList(fields, overrideField.RecordType);
                                break;
                            }
                        }
                    }
                    else if (!fields.Contains(field.Name.FullName))
                    {
                        //CreateConstraints(field);

                        fields.Add(field);
                    }
                }
            }
        }

        private void BuildBaseTypeList(RecordTypeCollection typeList, RecordType recordType)
        {
            typeList.Insert(0, recordType);

            if (recordType.BaseType != null)
            {
                BuildBaseTypeList(typeList, recordType.BaseType);
            }
        }

        private void BuildBaseTypeList(DataTypeCollection typeList, MidiDeviceSchemaDataType dataType)
        {
            if (!dataType.IsUnion)
            {
                typeList.Insert(0, dataType);

                if (dataType.HasBaseTypes)
                {
                    foreach (MidiDeviceSchemaDataType baseType in dataType.BaseTypes)
                    {
                        BuildBaseTypeList(typeList, baseType);
                    }
                }
            }
        }

        #endregion Flattend Field List methods
    }
}