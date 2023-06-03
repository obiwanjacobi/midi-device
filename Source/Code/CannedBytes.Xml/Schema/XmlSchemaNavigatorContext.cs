using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace CannedBytes.Xml.Schema
{
    public class XmlSchemaNavigationContext
    {
        public XmlSchemaNavigationContext()
        {
            InitializeTypeHandlers();
        }

        public virtual XmlSchemaNavigationContext CreateCopy(bool includeEventHandlers)
        {
            XmlSchemaNavigationContext target = new XmlSchemaNavigationContext();

            CopyTo(target, includeEventHandlers);

            return target;
        }

        public virtual void CopyTo(XmlSchemaNavigationContext target, bool includeEventHandlers)
        {
            target._lastAnnotation = _lastAnnotation;
            target._lastAttrGroup = _lastAttrGroup;
            target._lastAttribute = _lastAttribute;
            target._lastComplexType = _lastComplexType;
            target._lastElement = _lastElement;
            target._lastGroup = _lastGroup;
            target._lastNotation = _lastNotation;
            target._lastSimpleType = _lastSimpleType;

            if (includeEventHandlers)
            {
                target.NavigateAnnotation = NavigateAnnotation;
                target.NavigateAttribute = NavigateAttribute;
                target.NavigateAttributeGroup = NavigateAttributeGroup;
                target.NavigateComplexType = NavigateComplexType;
                target.NavigateElement = NavigateElement;
                target.NavigateGroup = NavigateGroup;
                target.NavigateNotation = NavigateNotation;
                target.NavigateSimpleType = NavigateSimpleType;
                target.NavigateComplexContentExtension = NavigateComplexContentExtension;
                target.NavigateComplexContentRestriction = NavigateComplexContentRestriction;
                target.NavigateSimpleContentExtension = NavigateSimpleContentExtension;
                target.NavigateSimpleContentRestriction = NavigateSimpleContentRestriction;
                target.NavigateSimpleTypeRestriction = NavigateSimpleTypeRestriction;
                target.NavigateSimpleTypeUnion = NavigateSimpleTypeUnion;
            }
        }

        public virtual void Reset()
        {
            _lastAnnotation = null;
            _lastAttrGroup = null;
            _lastAttribute = null;
            _lastComplexType = null;
            _lastElement = null;
            _lastGroup = null;
            _lastNotation = null;
            _lastSimpleType = null;
        }

        private void InitializeTypeHandlers()
        {
            TypeHandlers.Add(typeof(XmlSchemaImport).FullName, new XmlSchemaTypeHandler<XmlSchemaImport>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaInclude).FullName, new XmlSchemaTypeHandler<XmlSchemaInclude>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaAnnotation).FullName, new XmlSchemaTypeHandler<XmlSchemaAnnotation>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaAttribute).FullName, new XmlSchemaTypeHandler<XmlSchemaAttribute>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaAttributeGroup).FullName, new XmlSchemaTypeHandler<XmlSchemaAttributeGroup>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaComplexType).FullName, new XmlSchemaTypeHandler<XmlSchemaComplexType>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaSimpleType).FullName, new XmlSchemaTypeHandler<XmlSchemaSimpleType>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaElement).FullName, new XmlSchemaTypeHandler<XmlSchemaElement>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaGroup).FullName, new XmlSchemaTypeHandler<XmlSchemaGroup>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaNotation).FullName, new XmlSchemaTypeHandler<XmlSchemaNotation>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaComplexContentExtension).FullName, new XmlSchemaTypeHandler<XmlSchemaComplexContentExtension>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaComplexContentRestriction).FullName, new XmlSchemaTypeHandler<XmlSchemaComplexContentRestriction>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaSimpleContentExtension).FullName, new XmlSchemaTypeHandler<XmlSchemaSimpleContentExtension>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaSimpleContentRestriction).FullName, new XmlSchemaTypeHandler<XmlSchemaSimpleContentRestriction>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaSimpleTypeRestriction).FullName, new XmlSchemaTypeHandler<XmlSchemaSimpleTypeRestriction>(NavigateOn));
            TypeHandlers.Add(typeof(XmlSchemaSimpleTypeUnion).FullName, new XmlSchemaTypeHandler<XmlSchemaSimpleTypeUnion>(NavigateOn));
        }

        private Dictionary<String, MulticastDelegate> _typeHandlerMap;

        protected Dictionary<String, MulticastDelegate> TypeHandlers
        {
            get
            {
                if (_typeHandlerMap == null)
                {
                    _typeHandlerMap = new Dictionary<String, MulticastDelegate>();
                }

                return _typeHandlerMap;
            }
        }

        private XmlSchemaImport _lastImport;

        public XmlSchemaImport LastImport
        {
            get { return _lastImport; }
        }

        private XmlSchemaInclude _lastInclude;

        public XmlSchemaInclude LastInclude
        {
            get { return _lastInclude; }
        }

        private XmlSchemaAnnotation _lastAnnotation;

        public XmlSchemaAnnotation LastAnnotation
        {
            get { return _lastAnnotation; }
        }

        private XmlSchemaAttribute _lastAttribute;

        public XmlSchemaAttribute LastAttribute
        {
            get { return _lastAttribute; }
        }

        private XmlSchemaAttributeGroup _lastAttrGroup;

        public XmlSchemaAttributeGroup LastAttributeGroup
        {
            get { return _lastAttrGroup; }
        }

        private XmlSchemaComplexType _lastComplexType;

        public XmlSchemaComplexType LastComplextType
        {
            get { return _lastComplexType; }
        }

        private XmlSchemaSimpleType _lastSimpleType;

        public XmlSchemaSimpleType LastSimpleType
        {
            get { return _lastSimpleType; }
        }

        private XmlSchemaElement _lastElement;

        public XmlSchemaElement LastElement
        {
            get { return _lastElement; }
        }

        private XmlSchemaGroup _lastGroup;

        public XmlSchemaGroup LastGroup
        {
            get { return _lastGroup; }
        }

        private XmlSchemaNotation _lastNotation;

        public XmlSchemaNotation LastNotation
        {
            get { return _lastNotation; }
        }

        protected delegate void XmlSchemaTypeHandler<T>(T xmlObject);

        public bool Dispatch(XmlSchemaObject xmlObject)
        {
            MulticastDelegate del = null;
            string xmlObjectType = xmlObject.GetType().FullName;

            if (TypeHandlers.ContainsKey(xmlObjectType))
            {
                del = TypeHandlers[xmlObjectType];

                if (del != null)
                {
                    del.DynamicInvoke(xmlObject);
                }
            }

            return (del != null);
        }

        protected virtual void NavigateOn(XmlSchemaImport import)
        {
            if (NavigateImport != null)
            {
                NavigateImport(this, new XmlSchemaNavigatorEventArgs<XmlSchemaImport>(import));
            }

            _lastImport = import;
        }

        protected virtual void NavigateOn(XmlSchemaInclude include)
        {
            if (NavigateInclude != null)
            {
                NavigateInclude(this, new XmlSchemaNavigatorEventArgs<XmlSchemaInclude>(include));
            }

            _lastInclude = include;
        }

        protected virtual void NavigateOn(XmlSchemaAnnotation annotation)
        {
            if (NavigateAnnotation != null)
            {
                NavigateAnnotation(this, new XmlSchemaNavigatorEventArgs<XmlSchemaAnnotation>(annotation));
            }

            _lastAnnotation = annotation;
        }

        protected virtual void NavigateOn(XmlSchemaAttribute attribute)
        {
            if (NavigateAttribute != null)
            {
                NavigateAttribute(this, new XmlSchemaNavigatorEventArgs<XmlSchemaAttribute>(attribute));
            }

            _lastAttribute = attribute;
        }

        protected virtual void NavigateOn(XmlSchemaAttributeGroup attrGroup)
        {
            if (NavigateAttributeGroup != null)
            {
                NavigateAttributeGroup(this, new XmlSchemaNavigatorEventArgs<XmlSchemaAttributeGroup>(attrGroup));
            }

            _lastAttrGroup = attrGroup;
        }

        protected virtual void NavigateOn(XmlSchemaComplexType complexType)
        {
            if (NavigateComplexType != null)
            {
                NavigateComplexType(this, new XmlSchemaNavigatorEventArgs<XmlSchemaComplexType>(complexType));
            }

            _lastComplexType = complexType;
        }

        protected virtual void NavigateOn(XmlSchemaSimpleType simpleType)
        {
            if (NavigateSimpleType != null)
            {
                NavigateSimpleType(this, new XmlSchemaNavigatorEventArgs<XmlSchemaSimpleType>(simpleType));
            }

            _lastSimpleType = simpleType;
        }

        protected virtual void NavigateOn(XmlSchemaElement element)
        {
            if (NavigateElement != null)
            {
                NavigateElement(this, new XmlSchemaNavigatorEventArgs<XmlSchemaElement>(element));
            }

            _lastElement = element;
        }

        protected virtual void NavigateOn(XmlSchemaGroup group)
        {
            if (NavigateGroup != null)
            {
                NavigateGroup(this, new XmlSchemaNavigatorEventArgs<XmlSchemaGroup>(group));
            }

            _lastGroup = group;
        }

        protected virtual void NavigateOn(XmlSchemaNotation notation)
        {
            if (NavigateNotation != null)
            {
                NavigateNotation(this, new XmlSchemaNavigatorEventArgs<XmlSchemaNotation>(notation));
            }

            _lastNotation = notation;
        }

        protected virtual void NavigateOn(XmlSchemaComplexContentExtension complexExtension)
        {
            if (NavigateComplexContentExtension != null)
            {
                NavigateComplexContentExtension(this, new XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentExtension>(complexExtension));
            }
        }

        protected virtual void NavigateOn(XmlSchemaComplexContentRestriction complexRestriction)
        {
            if (NavigateComplexContentRestriction != null)
            {
                NavigateComplexContentRestriction(this, new XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentRestriction>(complexRestriction));
            }
        }

        protected virtual void NavigateOn(XmlSchemaSimpleContentExtension simpleExtension)
        {
            if (NavigateSimpleContentExtension != null)
            {
                NavigateSimpleContentExtension(this, new XmlSchemaNavigatorEventArgs<XmlSchemaSimpleContentExtension>(simpleExtension));
            }
        }

        protected virtual void NavigateOn(XmlSchemaSimpleContentRestriction simpleRestriction)
        {
            if (NavigateSimpleContentRestriction != null)
            {
                NavigateSimpleContentRestriction(this, new XmlSchemaNavigatorEventArgs<XmlSchemaSimpleContentRestriction>(simpleRestriction));
            }
        }

        protected virtual void NavigateOn(XmlSchemaSimpleTypeRestriction simpleRestriction)
        {
            if (NavigateSimpleTypeRestriction != null)
            {
                NavigateSimpleTypeRestriction(this, new XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeRestriction>(simpleRestriction));
            }
        }

        protected virtual void NavigateOn(XmlSchemaSimpleTypeUnion simpleUnion)
        {
            if (NavigateSimpleTypeUnion != null)
            {
                NavigateSimpleTypeUnion(this, new XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeUnion>(simpleUnion));
            }
        }

        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaImport>> NavigateImport;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaInclude>> NavigateInclude;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaAnnotation>> NavigateAnnotation;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaAttribute>> NavigateAttribute;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaAttributeGroup>> NavigateAttributeGroup;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaComplexType>> NavigateComplexType;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaElement>> NavigateElement;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaGroup>> NavigateGroup;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaNotation>> NavigateNotation;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaSimpleType>> NavigateSimpleType;
        // not backed by a last property
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentExtension>> NavigateComplexContentExtension;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentRestriction>> NavigateComplexContentRestriction;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaSimpleContentExtension>> NavigateSimpleContentExtension;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaSimpleContentRestriction>> NavigateSimpleContentRestriction;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeRestriction>> NavigateSimpleTypeRestriction;
        public event EventHandler<XmlSchemaNavigatorEventArgs<XmlSchemaSimpleTypeUnion>> NavigateSimpleTypeUnion;
    }

    public class XmlSchemaNavigatorEventArgs<T> : EventArgs
    {
        public XmlSchemaNavigatorEventArgs(T xmlObject)
        {
            _xmlObject = xmlObject;
        }

        private T _xmlObject;

        public T XmlObject
        {
            get { return _xmlObject; }
        }
    }
}