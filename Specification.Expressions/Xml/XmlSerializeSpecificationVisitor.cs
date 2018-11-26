namespace Specification.Expressions.Xml
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

        public override Specification VisitAnd(AndSpecification and)
        {
            this.XmlWriter.WriteStartElement(Consts.And, this.NameSpace);
            var result = base.VisitAnd(and);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override Specification VisitOr(OrSpecification or)
        {
            this.XmlWriter.WriteStartElement(Consts.Or, this.NameSpace);
            var result = base.VisitOr(or);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override Specification VisitConstant(ConstantSpecification cs)
        {
            this.XmlWriter.WriteStartElement(cs.Value.ToString().ToLowerInvariant(), this.NameSpace);

            this.XmlWriter.WriteEndElement();

            return cs;
        }

        public override Specification VisitHasValue(HasValueSpecification hv)
        {
            this.XmlWriter.WriteStartElement(Consts.HasValue, this.NameSpace);
            this.WriteKey(hv.Key);
            this.XmlWriter.WriteEndElement();
            return hv;
        }

        public override Specification VisitNot(NotSpecification not)
        {
            this.XmlWriter.WriteStartElement(Consts.Not, this.NameSpace);

            var result = base.VisitNot(not);
            this.XmlWriter.WriteEndElement();

            return result;
        }

        public override Specification VisitEqual(EqualSpecification eq)
        {
            this.WriteKeyValueSpecification(eq, Consts.Eq);

            return eq;
        }

        public override Specification VisitGreater(GreaterSpecification gt)
        {
            this.WriteKeyValueSpecification(gt, Consts.Gt);

            return gt;
        }

        public override Specification VisitGreaterOrEqual(GreaterOrEqualSpecification ge)
        {
            this.WriteKeyValueSpecification(ge, Consts.Ge);

            return ge;
        }

        public override Specification VisitLess(LessSpecification lt)
        {
            this.WriteKeyValueSpecification(lt, Consts.Lt);

            return lt;
        }

        public override Specification VisitLessOrEqual(LessOrEqualSpecification le)
        {
            this.WriteKeyValueSpecification(le, Consts.Le);

            return le;
        }

        public override Specification VisitReference(ReferenceSpecification rf)
        {
            this.XmlWriter.WriteStartElement(Consts.Ref, this.NameSpace);
            this.WriteKey(rf.Key);

            this.XmlWriter.WriteEndElement();
            return rf;
        }

        private void WriteKeyValueSpecification(KeyValueSpecification specification, string name)
        {
            this.XmlWriter.WriteStartElement(name, this.NameSpace);
            this.WriteKey(specification.Key);
            this.WriteValue(specification.Value);

            this.XmlWriter.WriteEndElement();
        }

        private void WriteKey(string key)
        {
            this.XmlWriter.WriteAttributeString(Consts.Key, key);
        }

        public static void WriteValue(XmlWriter writer, SpecificationValue value)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (value.IsReference)
            {
                writer.WriteAttributeString(Consts.ValueRef, value.Values.Single().ToString());
                return;
            }

            if (value.ValueType != SpecificationValue.DataType.String)
            {
                writer.WriteAttributeString(Consts.Type, value.ValueType.ToString("G").ToLowerInvariant());
            }

            if (value.ValueMultiplicity != SpecificationValue.Multiplicity.AnyOf && value.Values.Count > 1)
            {
                writer.WriteAttributeString(Consts.Mul, value.ValueMultiplicity.ToString("G").ToLowerInvariant());
            }

            if (value.Values.Count > 1)
            {
                for (int i = 0; i < value.Values.Count; i++)
                {
                    writer.WriteElementString(Consts.Value, value.Serialize(i));
                }
            }
            else
            {
                writer.WriteAttributeString(Consts.Value, value.Serialize());
            }
        }

        private void WriteValue(SpecificationValue value)
        {
            WriteValue(this.XmlWriter, value);
        }
    }
}