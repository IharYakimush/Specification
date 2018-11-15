namespace Specification.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    using global::Specification.Expressions.Operators;

    public static class XmlParserExtensions
    {
        public static ConcurrentDictionary<string, XmlSchemaSet> Schemas { get; } =
            new ConcurrentDictionary<string, XmlSchemaSet>();

        public static ConcurrentDictionary<int, SpecificationValueSettings> ValueSettings { get; } =
            new ConcurrentDictionary<int, SpecificationValueSettings>();

        public static Specification FromXml(this SpecificationParser parser, XElement element, string ns = null)
        {
            ns = ns ?? string.Empty;

            XmlSchemaSet schemaSet = Schemas.GetOrAdd(
                ns,
                nss =>
                    {
                        XmlSchemaSet result = new XmlSchemaSet();
                        Assembly assembly = typeof(XmlParserExtensions).Assembly;
                        string manifest = assembly.GetManifestResourceNames().SingleOrDefault(s => s.Contains(".xsd"));

                        Stream stream = assembly.GetManifestResourceStream(manifest);

                        result.Add(ns, new XmlTextReader(stream));
                        result.Compile();
                        return result;
                    });

            XmlSchema schema = schemaSet.Schemas(ns).OfType<XmlSchema>().First();

            string name = element.Name.LocalName;
            XmlQualifiedName qualifiedName = new XmlQualifiedName(name, ns);

            if (schema.Elements.Contains(qualifiedName))
            {
                element.Validate(
                    schema.Elements[qualifiedName],
                    schemaSet,
                    (sender, args) => throw new ArgumentException(args.Message, args.Exception));
            }
            else
            {
                throw new ArgumentException(string.Format(SpecAbsRes.XmlParseUnknownElement, qualifiedName));
            }

            return FromXmlValidated(element, schema, schemaSet, ns);
        }

        private static Specification FromXmlValidated(XElement element, XmlSchema schema, XmlSchemaSet schemaSet, string ns)
        {            
            if (element.Name.LocalName == Consts.And || element.Name.LocalName == Consts.Or)
            {
                List<Specification> inner = new List<Specification>(element.Elements().Count(el => el.Name.Namespace == ns));

                foreach (XElement item in element.Elements().Where(el => el.Name.Namespace == ns))
                {
                    inner.Add(FromXmlValidated(item, schema, schemaSet, ns));
                }

                if (element.Name.LocalName == Consts.And)
                {
                    return new AndSpecification(inner);
                }

                if (element.Name.LocalName == Consts.Or)
                {
                    return new OrSpecification(inner);
                }
            }

            if (element.Name.LocalName == Consts.Not)
            {
                Specification inner = FromXmlValidated(element.Elements().Single(), schema, schemaSet, ns);
                return new NotSpecification(inner);
            }

            if (element.Name.LocalName == Consts.HasValue)
            {
                return new HasValueSpecification(GetKey(element, ns));
            }

            if (element.Name.LocalName == Consts.Ref)
            {
                return new ReferenceSpecification(GetKey(element, ns));
            }

            if (element.Name.LocalName == Consts.True)
            {
                return ConstantSpecification.True;
            }

            if (element.Name.LocalName == Consts.False)
            {
                return ConstantSpecification.False;
            }

            if (Consts.CompareOperators.Contains(element.Name.LocalName))
            {
                string key = GetKey(element, ns);
                bool isRef = GetRef(element, ns);
                SpecificationValue.Multiplicity mul = GetMul(element, ns);
                SpecificationValue.DataType type = GetType(element, ns);
                string[] values = GetValues(element, ns).ToArray();
                SpecificationValue value;

                if (isRef)
                {
                    element.Validate(
                        schema.SchemaTypes[new XmlQualifiedName("valueReference", ns)],
                        schemaSet,
                        (sender, args) => throw new ArgumentException(args.Message, args.Exception));

                    value = SpecificationValue.Ref(values.Single());
                }
                else
                {
                    if (values.Length <= 1)
                    {
                        element.Validate(
                            schema.SchemaTypes[new XmlQualifiedName("valueSingle", ns)],
                            schemaSet,
                            (sender, args) => throw new ArgumentException(args.Message, args.Exception));
                    }
                    else
                    {
                        element.Validate(
                            schema.SchemaTypes[new XmlQualifiedName("valueMultiple", ns)],
                            schemaSet,
                            (sender, args) => throw new ArgumentException(args.Message, args.Exception));
                    }

                    SpecificationValueSettings settings = ValueSettings.GetOrAdd(
                        (int)mul * 1000 + (int)type,
                        i => new SpecificationValueSettings
                                 {
                                     AllowCast = true,
                                     DefaultMultiplicity = mul,
                                     IncludeDetails = true,
                                     ExpectedType = type
                                 });

                    if (!SpecificationValue.TryFrom(
                            values,
                            settings,
                            out value,
                            out string error))
                    {
                        throw new ArgumentException(error);
                    }
                }
                
                if (element.Name.LocalName == Consts.Eq)
                {
                    return new EqualSpecification(key, value);
                }

                if (element.Name.LocalName == Consts.Gt)
                {
                    return new GreaterSpecification(key, value);
                }

                if (element.Name.LocalName == Consts.Ge)
                {
                    return new GreaterOrEqualSpecification(key, value);
                }

                if (element.Name.LocalName == Consts.Lt)
                {
                    return new LessSpecification(key, value);
                }

                if (element.Name.LocalName == Consts.Le)
                {
                    return new LessOrEqualSpecification(key, value);
                }
            }

            throw new NotImplementedException();
        }

        private static string GetKey(XElement element, string ns)
        {
            return element.Attributes().Single(at => at.Name.LocalName == Consts.Key && (at.Name.Namespace == ns || at.Name.Namespace == string.Empty)).Value;
        }

        private static bool GetRef(XElement element, string ns)
        {
            string value = element.Attributes().SingleOrDefault(at => at.Name.LocalName == Consts.Ref && (at.Name.Namespace == ns || at.Name.Namespace == string.Empty))?.Value;

            return value != null && bool.Parse(value);
        }

        private static SpecificationValue.DataType GetType(XElement element, string ns)
        {
            string value = element.Attributes().SingleOrDefault(at => at.Name.LocalName == Consts.Type && (at.Name.Namespace == ns || at.Name.Namespace == string.Empty))?.Value;

            return value == null
                       ? SpecificationValue.DataType.String
                       : (SpecificationValue.DataType)Enum.Parse(typeof(SpecificationValue.DataType), value, true);
        }

        private static SpecificationValue.Multiplicity GetMul(XElement element, string ns)
        {
            string value = element.Attributes().SingleOrDefault(at => at.Name.LocalName == Consts.Mul && (at.Name.Namespace == ns || at.Name.Namespace == string.Empty))?.Value;

            return value == null
                       ? SpecificationValue.Multiplicity.AnyOf
                       : (SpecificationValue.Multiplicity)Enum.Parse(
                           typeof(SpecificationValue.Multiplicity),
                           value,
                           true);
        }

        private static IEnumerable<string> GetValues(XElement element, string ns)
        {
            XAttribute value = element.Attributes()
                .SingleOrDefault(at => at.Name.LocalName == Consts.Value && (at.Name.Namespace == ns || at.Name.Namespace == string.Empty));

            if (value != null)
            {
                yield return value.Value;
            }
            else
            {
                foreach (var item in element.Elements().Where(el => el.Name.Namespace == ns))
                {
                    yield return item.Value;
                }
            }
        }
    }
}