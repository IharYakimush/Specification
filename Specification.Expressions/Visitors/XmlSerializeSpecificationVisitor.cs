namespace Specification.Expressions.Visitors
{
    using System;
    using System.Linq;
    using System.Xml;

    using global::Specification.Expressions.Operators;

    public class XmlSerializeSpecificationVisitor : SpecificationVisitor
    {
        public XmlWriter XmlWriter { get; }

        public string NameSpace { get; }

        public XmlSerializeSpecificationVisitor(XmlWriter xmlWriter, string nameSpace = null)
        {
            this.XmlWriter = xmlWriter ?? throw new ArgumentNullException(nameof(xmlWriter));
            this.NameSpace = nameSpace ?? string.Empty;
        }

        public override AndSpecification VisitAnd(AndSpecification and)
        {
            this.XmlWriter.WriteStartElement("and", this.NameSpace);
            var result = base.VisitAnd(and);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override OrSpecification VisitOr(OrSpecification or)
        {
            this.XmlWriter.WriteStartElement("or", this.NameSpace);
            var result = base.VisitOr(or);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override ConstantSpecification VisitConstant(ConstantSpecification cs)
        {
            this.XmlWriter.WriteStartElement(cs.Value.ToString().ToLowerInvariant(), this.NameSpace);

            this.XmlWriter.WriteEndElement();

            return cs;
        }

        public override HasValueSpecification VisitHasValue(HasValueSpecification hv)
        {
            this.XmlWriter.WriteStartElement("hasvalue", this.NameSpace);
            this.WriteKey(hv);
            this.XmlWriter.WriteEndElement();
            return hv;
        }

        public override NotSpecification VisitNot(NotSpecification not)
        {
            this.XmlWriter.WriteStartElement("not", this.NameSpace);

            var result = base.VisitNot(not);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override EqualSpecification VisitEqual(EqualSpecification eq)
        {
            this.XmlWriter.WriteStartElement("eq", this.NameSpace);
            this.WriteKey(eq);
            this.WriteValue(eq.Value);

            this.XmlWriter.WriteEndElement();

            return eq;
        }

        private void WriteKey(KeySpecification kv)
        {
            this.XmlWriter.WriteAttributeString("k", kv.Key);
        }

        private void WriteValue(SpecificationValue value)
        {
            if (value.ValueType != SpecificationValue.DataType.String)
            {
                this.XmlWriter.WriteAttributeString("t", value.ValueType.ToString("G").ToLowerInvariant());
            }

            if (value.ValueMultiplicity != SpecificationValue.Multiplicity.AnyOf && value.Values.Count > 1)
            {
                this.XmlWriter.WriteAttributeString("m", value.ValueMultiplicity.ToString("G").ToLowerInvariant());
            }

            if (value.Values.Count > 1)
            {
                for (int i = 0; i < value.Values.Count; i++)
                {
                    this.XmlWriter.WriteElementString("v", value.Serialize(i));
                }                
            }
            else
            {
                this.XmlWriter.WriteAttributeString("v", value.Serialize());
            }
        }
    }
}