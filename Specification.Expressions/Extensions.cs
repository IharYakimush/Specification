namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    using global::Specification.Expressions.Visitors;

    public static class Extensions
    {
        public static SpecificationResult Evaluate(
            this Specification specification,
            IReadOnlyDictionary<string, object> values)
        {
            return specification.Evaluate(values, SpecificationEvaluationSettings.Default);
        }

        public static void ToXml(this Specification specification, XmlWriter writer, string ns = null)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            XmlSerializeSpecificationVisitor visitor = new XmlSerializeSpecificationVisitor(writer, ns);
            visitor.Visit(specification);
        }

        public static string ToXml(this Specification specification, string ns = null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(
                    ms,
                    new XmlWriterSettings
                        {
                            CheckCharacters = true,
                            Encoding = Encoding.UTF8,
                            Indent = true
                        }))
                {
                    specification.ToXml(writer, ns);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}