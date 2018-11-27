# Specification
Filter data using specification expressions tree with following advantages:
- ability load specification from xml. json format support comming soon.
- concept of "specification reference" and "value reference" to reuse already defined expressions in more complex ones.
- comparison between single and multiple values
- base visitors for efficien work with specification expressions tree

## Getting started
Define specification expression
```
Specification specification = new AndSpecification(
    new EqualSpecification("key1", SpecificationValue.Single("value1")),
    new OrSpecification(
        new HasValueSpecification("key2"),
		new EqualSpecification("key5", SpecificationValue.Single(new DateTime(2018, 10, 10, 1, 2, 3))),

        // value of key3 should be equal to any of values 1,2,3
        new EqualSpecification("key3", SpecificationValue.AnyOf(1, 2, 3))),

    // key4 should be equal to value with name "currentDateTime" which will be resolved at specification evaluation
    new EqualSpecification("key4", SpecificationValue.Ref("currentDateTime")));
```
Evaluate specification expression
```
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
```
Serialize specification expression to xml `string xml = specification.ToXml();`. DateTime values will be converted to UTC for serialization and comparison.
```
<and>
  <eq key="key1" value="value1" />
  <or>
    <hasvalue key="key2" />
	<eq key="key5" t="datetime" value="2018-10-09 22:02:03Z" />
    <eq key="key3" t="int">
      <value>1</value>
      <value>2</value>
      <value>3</value>
    </eq>
  </or>
  <ge key="key4" valueRef="currentDateTime" />
</and>
```
Parse specification from xml `Specification sp2 = Specification.Parse.FromXml(XElement.Parse(xml));`. Parse operation assume DateTime to be UTC values.

## NuGet
https://www.nuget.org/packages/Specification.Expressions

## Contribution
Please feel free to create issues and pool requests to develop branch