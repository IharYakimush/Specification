namespace Specification.Expressions
{
    using System.Collections.Generic;
    using System.Dynamic;

    public static class Consts
    {
        public const string And = "and";
        public const string Or = "or";
        public const string Not = "not";
        public const string Eq = "eq";
        public const string Gt = "gt";
        public const string Ge = "ge";
        public const string Lt = "lt";
        public const string Le = "le";
        public const string True = "true";
        public const string False = "false";
        public const string HasValue = "hasvalue";
        public const string Ref = "ref";
        public const string ValueRef = "valueRef";
        public const string Key = "key";
        public const string Value = "value";
        public const string Type = "t";
        public const string Mul = "m";
        public const string Add = "add";
        public const string Runtime = "runtime";
        public const string CollectionRoot = "specCollection";
        public const string RefValuesCollection = "values";
        public const string RefSpecCollection = "definitions";
        public const string SpecCollection = "specifications";

        public static HashSet<string> CompareOperators { get; } = new HashSet<string> { Eq, Gt, Ge, Lt, Le };
    }
}