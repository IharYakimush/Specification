namespace Specification.Expressions.Xml
{
    using System;
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
            this.XmlWriter.WriteStartElement(Consts.And, this.NameSpace);
            var result = base.VisitAnd(and);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override OrSpecification VisitOr(OrSpecification or)
        {
            this.XmlWriter.WriteStartElement(Consts.Or, this.NameSpace);
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
            this.XmlWriter.WriteStartElement(Consts.HasValue, this.NameSpace);
            this.WriteKey(hv);
            this.XmlWriter.WriteEndElement();
            return hv;
        }

        public override NotSpecification VisitNot(NotSpecification not)
        {
            this.XmlWriter.WriteStartElement(Consts.Not, this.NameSpace);

            var result = base.VisitNot(not);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override EqualSpecification VisitEqual(EqualSpecification eq)
        {
            this.WriteKeyValueSpecification(eq, Consts.Eq);

            return eq;
        }

        public override GreaterSpecification VisitGreater(GreaterSpecification gt)
        {
            this.WriteKeyValueSpecification(gt, Consts.Gt);

            return gt;
        }

        public override GreaterOrEqualSpecification VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            this.WriteKeyValueSpecification(ge, Consts.Ge);

            return ge;
        }

        public override LessSpecification VisitLess(LessSpecification lt)
        {
            this.WriteKeyValueSpecification(lt, Consts.Lt);

            return lt;
        }

        public override LessOrEqualSpecification VisitLessOrEqual(LessOrEqualSpecification le)
        {
            this.WriteKeyValueSpecification(le, Consts.Le);

            return le;
        }

        private void WriteKeyValueSpecification(KeyValueSpecification specification, string name)
        {
            this.XmlWriter.WriteStartElement(name, this.NameSpace);
            this.WriteKey(specification);
            this.WriteValue(specification.Value);

            this.XmlWriter.WriteEndElement();
        }

        private void WriteKey(KeySpecification kv)
        {
            this.XmlWriter.WriteAttributeString(Consts.Key, kv.Key);
        }

        private void WriteValue(SpecificationValue value)
        {            
            if (value.ValueType != SpecificationValue.DataType.String)
            {
                this.XmlWriter.WriteAttributeString(Consts.Type, value.ValueType.ToString("G").ToLowerInvariant());
            }

            if (value.ValueMultiplicity != SpecificationValue.Multiplicity.AnyOf && value.Values.Count > 1)
            {
                this.XmlWriter.WriteAttributeString(Consts.Mul, value.ValueMultiplicity.ToString("G").ToLowerInvariant());
            }

            if (value.Values.Count > 1)
            {
                for (int i = 0; i < value.Values.Count; i++)
                {
                    this.XmlWriter.WriteElementString(Consts.Value, value.Serialize(i));
                }                
            }
            else
            {
                this.XmlWriter.WriteAttributeString(Consts.Value, value.Serialize());
            }

            if (value.IsReference)
            {
                this.XmlWriter.WriteAttributeString(Consts.Ref, bool.TrueString.ToLowerInvariant());
            }
        }
    }
}