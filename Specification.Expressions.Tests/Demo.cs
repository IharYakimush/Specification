namespace Specification.Expressions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using global::Specification.Expressions.Operators;

    using Xunit;

    public class Demo
    {
        [Fact]
        public void DoSomthing()
        {
            Specification specification = new AndSpecification(
                new EqualSpecification("key1", SpecificationValue.Single("value1")),
                new OrSpecification(
                    new HasValueSpecification("key2"),

                    // value of key3 should be equal to any of values 1,2,3
                    new EqualSpecification("key3", SpecificationValue.AnyOf(1, 2, 3))),

                // key4 should be greater or equal to value with name "currentDateTime" which will be resolved at specification evaluation
                new GreaterOrEqualSpecification("key4", SpecificationValue.Ref("currentDateTime")));

            Dictionary<string, object> values = new Dictionary<string, object>
                                                    {
                                                        { "key1", "value1" },
                                                        { "key3", 1 },
                                                        { "key4", DateTime.Now.AddMinutes(1) },

                                                        // value referenced from specification
                                                        { "currentDateTime", DateTime.UtcNow }                                                        
                                                    };

            SpecificationResult result = specification.Evaluate(values);
            Assert.True(result.IsSatisfied);

            string xml = specification.ToXml();
            Console.WriteLine(xml);

            Specification sp2 = Specification.Parse.FromXml(XElement.Parse(xml));
        }
    }
}