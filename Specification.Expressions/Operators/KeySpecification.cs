﻿namespace Specification.Expressions.Operators
{
    using System;
    using System.Collections.Generic;

    public abstract class KeySpecification : Specification
    {
        public string Key { get; }

        protected KeySpecification(string key)
        {
            this.Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}