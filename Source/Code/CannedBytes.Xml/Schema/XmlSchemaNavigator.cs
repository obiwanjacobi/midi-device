using System.Xml.Schema;

namespace CannedBytes.Xml.Schema
{
    public class XmlSchemaNavigator
    {
        public XmlSchemaNavigator(XmlSchema schema)
        {
            Check.IfArgumentNull(schema, "schema");

            _schema = schema;
        }

        private XmlSchema _schema;

        /// <summary>
        /// Gets the XmlSchema the navigator uses.
        /// </summary>
        public XmlSchema XmlSchema
        {
            get { return _schema; }
        }

        /// <summary>
        /// Navigates Includes and Items
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int NavigateSchema(XmlSchemaNavigationContext context)
        {
            Check.IfArgumentNull(context, "context");

            int result = NavigateCollection(context, XmlSchema.Includes);
            result += NavigateCollection(context, XmlSchema.Items);

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public int NavigateElements(XmlSchemaNavigationContext context, XmlSchemaComplexType complexType)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(complexType, "complexType");

            int result = 0;

            if (complexType.Particle != null)
            {
                result = NavigateParticle(context, complexType.Particle);
            }
            else
            {
                result = NavigateBaseTypeElements(context, complexType);
            }

            return result;
        }

        public int NavigateParticle(XmlSchemaNavigationContext context, XmlSchemaParticle particle)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(particle, "particle");

            int result = 0;

            XmlSchemaSequence sequence = particle as XmlSchemaSequence;

            if (sequence != null)
            {
                result = NavigateCollection(context, sequence.Items);
            }

            XmlSchemaChoice choice = particle as XmlSchemaChoice;

            if (choice != null)
            {
                result = NavigateCollection(context, choice.Items);
            }

            return result;
        }

        public int NavigateBaseType(XmlSchemaNavigationContext context, XmlSchemaComplexType complexType)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(complexType, "complexType");

            if (complexType.ContentModel != null)
            {
                XmlSchemaComplexContent complexContent =
                    complexType.ContentModel as XmlSchemaComplexContent;

                if (complexContent != null)
                {
                    context.Dispatch(complexContent.Content);
                }

                XmlSchemaSimpleContent simpleContent =
                    complexType.ContentModel as XmlSchemaSimpleContent;

                if (simpleContent != null)
                {
                    context.Dispatch(simpleContent.Content);
                }
            }

            return 0;
        }

        public int NavigateBaseType(XmlSchemaNavigationContext context, XmlSchemaSimpleType simpleType)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(simpleType, "simpleType");

            if (simpleType.Content != null)
            {
                context.Dispatch(simpleType.Content);
            }

            return 0;
        }

        public int NavigateBaseTypeElements(XmlSchemaNavigationContext context, XmlSchemaComplexType complexType)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(complexType, "complexType");

            XmlSchemaNavigationContext privateCtx = new XmlSchemaNavigationContext();
            int result = 0;

            privateCtx.NavigateComplexContentExtension += delegate(object extension,
                    XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentExtension> extensionEventArgs)
                {
                    if (extensionEventArgs.XmlObject.Particle != null)
                    {
                        result += NavigateParticle(context, extensionEventArgs.XmlObject.Particle);
                    }
                };

            privateCtx.NavigateComplexContentRestriction += delegate(object restriction,
                    XmlSchemaNavigatorEventArgs<XmlSchemaComplexContentRestriction> restrictionEventArgs)
                {
                    if (restrictionEventArgs.XmlObject.Particle != null)
                    {
                        result += NavigateParticle(context, restrictionEventArgs.XmlObject.Particle);
                    }
                };

            NavigateBaseType(privateCtx, complexType);

            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public int NavigateCollection(XmlSchemaNavigationContext context, XmlSchemaObjectCollection items)
        {
            Check.IfArgumentNull(context, "context");
            Check.IfArgumentNull(items, "items");

            int count = 0;

            foreach (XmlSchemaObject xmlObject in items)
            {
                if (context.Dispatch(xmlObject))
                {
                    count++;
                }
            }

            return count;
        }
    }
}