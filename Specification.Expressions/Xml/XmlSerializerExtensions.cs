namespace Specification.Expressions.Xml
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    using global::Specification.Expressions.Visitors;

    public static class XmlSerializerExtensions
    {
        public static void ToXml(this Specification specification, XmlWriter writer, string ns = null)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            XmlSerializeSpecificationVisitor visitor = new XmlSerializeSpecificationVisitor(writer, ns);
            visitor.Visit(specification);
        }

        public static string ToXml(this Specification specification, string ns = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(
                stringBuilder,
                new XmlWriterSettings
                    {
                        CheckCharacters = true,
                        Encoding = Encoding.UTF8,
                        Indent = true,
                        OmitXmlDeclaration = true
                    }))
            {
                specification.ToXml(writer, ns);
            }

            return stringBuilder.ToString();
        }
    }
}