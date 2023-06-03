using System;
using System.Diagnostics;
using System.Xml.Schema;

namespace CannedBytes.Midi.Device.Schema.Xml
{
    public abstract class MidiDeviceSchemaConstraint : Constraint
    {
        protected MidiDeviceSchemaConstraint(string name,
            ConstraintValidationType validationType)
            : base(name, validationType)
        { }

        public static MidiDeviceSchemaConstraint Create(XmlSchemaFacet facet)
        {
            Check.IfArgumentNull(facet, "facet");

            MidiDeviceSchemaConstraint constraint = null;

            switch (facet.GetType().Name)
            {
                case "XmlSchemaMaxInclusiveFacet":
                    constraint = new MaxInclusiveValueConstraint(
                        (XmlSchemaMaxInclusiveFacet)facet);
                    break;
                case "XmlSchemaMinInclusiveFacet":
                    constraint = new MinInclusiveValueConstraint(
                        (XmlSchemaMinInclusiveFacet)facet);
                    break;
                case "XmlSchemaEnumerationFacet":
                    constraint = new EnumeratedValueConstraint(
                        (XmlSchemaEnumerationFacet)facet);
                    break;
                case "XmlSchemaLengthFacet":
                    constraint = new LengthValueConstraint(
                        (XmlSchemaLengthFacet)facet);
                    break;
                default:
                    //throw new NotSupportedException(
                    //    String.Format("Xml Facet '{0}' is not supported.", facet.GetType().Name));
                    Debug.Write(String.Format("Xml Facet '{0}' is not supported.", facet.GetType().Name), "MidiDeviceSchemaConstraints");
                    break;
            }

            return constraint;
        }

        public static MidiDeviceSchemaConstraint Create(string fixedValue)
        {
            Check.IfArgumentNullOrEmpty(fixedValue, "fixedValue");

            return new FixedValueConstraint(fixedValue);
        }
    }

    internal abstract class XmlFacetConstraint<TValue> : MidiDeviceSchemaConstraint
        where TValue : IComparable
    {
        protected XmlFacetConstraint(string name,
            ConstraintValidationType validationType, XmlSchemaFacet facet)
            : base(name, validationType)
        {
            _xmlType = facet;
            _value = (TValue)Convert.ChangeType(facet.Value, typeof(TValue));
        }

        private XmlSchemaFacet _xmlType;

        public XmlSchemaFacet XmlType
        {
            get { return _xmlType; }
        }

        private TValue _value;

        public override T GetValue<T>()
        {
            return (T)Convert.ChangeType(_value, typeof(T));
        }
    }

    internal class MinInclusiveValueConstraint : XmlFacetConstraint<long>
    {
        public MinInclusiveValueConstraint(XmlSchemaMinInclusiveFacet facet)
            : base("MinInclusiveValueConstraint", ConstraintValidationType.All, facet)
        {
            ConstraintType = ConstraintType.MinInclusive;
        }

        public override bool Validate<T>(T data)
        {
            return (data.CompareTo(GetValue<T>()) >= 0);
        }
    }

    internal class MaxInclusiveValueConstraint : XmlFacetConstraint<long>
    {
        public MaxInclusiveValueConstraint(XmlSchemaMaxInclusiveFacet facet)
            : base("MaxInclusiveValueConstraint", ConstraintValidationType.All, facet)
        {
            ConstraintType = ConstraintType.MaxInclusive;
        }

        public override bool Validate<T>(T data)
        {
            return (data.CompareTo(GetValue<T>()) <= 0);
        }
    }

    internal class EnumeratedValueConstraint : XmlFacetConstraint<byte>
    {
        public EnumeratedValueConstraint(XmlSchemaEnumerationFacet facet)
            : this((XmlSchemaFacet)facet)
        {
            ConstraintType = ConstraintType.Enumeration;
        }

        private EnumeratedValueConstraint(XmlSchemaFacet facet)
            : base("EnumeratedValueConstraint", ConstraintValidationType.One, facet)
        { }

        public override bool Validate<T>(T data)
        {
            return (data.CompareTo(GetValue<T>()) == 0);
        }
    }

    internal class LengthValueConstraint : XmlFacetConstraint<int>
    {
        public LengthValueConstraint(XmlSchemaLengthFacet facet)
            : this((XmlSchemaFacet)facet)
        {
            ConstraintType = ConstraintType.FixedLength;
        }

        private LengthValueConstraint(XmlSchemaFacet facet)
            : base("LengthValueConstraint", ConstraintValidationType.One, facet)
        { }

        public override bool Validate<T>(T data)
        {
            // TODO: We need an interface to be able to validate multiple bytes as one value

            return true;
        }
    }

    internal class FixedValueConstraint : MidiDeviceSchemaConstraint
    {
        public FixedValueConstraint(string value)
            : base("FixedValueConstraint", ConstraintValidationType.One)
        {
            _value = Convert.ToInt64(value);
            ConstraintType = ConstraintType.FixedValue;
        }

        private long _value;

        public override T GetValue<T>()
        {
            return (T)Convert.ChangeType(_value, typeof(T));
        }

        public override bool Validate<T>(T data)
        {
            return (data.CompareTo(GetValue<T>()) == 0);
        }
    }
}