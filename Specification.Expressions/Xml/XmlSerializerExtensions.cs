namespace Specification.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Xml;

    public static class XmlSerializerExtensions
    {
        private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings
                                                                       {
                                                                           CheckCharacters = true,
                                                                           Encoding = Encoding.UTF8,
                                                                           Indent = true,
                                                                           OmitXmlDeclaration = true
                                                                       };
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
                writerSettings))
            {
                specification.ToXml(writer, ns);
            }

            return stringBuilder.ToString();
        }

        public static void ToXml(this SpecificationsCollection collection, XmlWriter writer, string ns = null)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            ns = ns ?? string.Empty;

            writer.WriteStartElement(Consts.CollectionRoot, ns);

            // Value references
            writer.WriteStartElement(Consts.RefValuesCollection, ns);
            foreach (var val in collection.ValuesForReference)
            {
                writer.WriteStartElement(Consts.Add, ns);
                writer.WriteAttributeString(Consts.Key, val.Key);
                XmlSerializeSpecificationVisitor.WriteValue(writer, val.Value);
                writer.WriteEndElement();
            }

            foreach (string runtimeValue in collection.AllowedRuntimeValueReferenceKeys.Except(
                collection.ValuesForReference.Keys))
            {
                writer.WriteStartElement(Consts.Runtime, ns);
                writer.WriteAttributeString(Consts.Key, runtimeValue);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            // Specification references
            writer.WriteStartElement(Consts.RefSpecCollection, ns);
            WriteSpecifications(collection.SpecificationsForReference, writer, ns);

            foreach (string runtimeSpec in collection.AllowedRuntimeSpecificationReferenceKeys.Except(
                collection.SpecificationsForReference.Keys))
            {
                writer.WriteStartElement(Consts.Runtime, ns);
                writer.WriteAttributeString(Consts.Key, runtimeSpec);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            // Main specifications
            writer.WriteStartElement(Consts.SpecCollection, ns);
            WriteSpecifications(collection.Specifications, writer, ns);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static void WriteSpecifications(IDictionary<string,Specification> specifications, XmlWriter writer, string ns)
        {
            foreach (var spec in specifications)
            {
                writer.WriteStartElement(Consts.Add, ns);
                writer.WriteAttributeString(Consts.Key, spec.Key);
                spec.Value.ToXml(writer, ns);
                writer.WriteEndElement();
            }
        }

        public static string ToXml(this SpecificationsCollection collection, string ns = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(
                stringBuilder,
                writerSettings))
            {
                collection.ToXml(writer, ns);
            }

            return stringBuilder.ToString();
        }
    }
}