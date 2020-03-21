using System;
using System.Collections.Generic;

namespace UTF.TestTools.Collections
{
    public class ComparerEx<T> : Comparer<T>
    {
        private readonly Comparison<T> _compareFunction;

        public ComparerEx(Comparison<T> comparison)
        {
            if (comparison == null)
                throw new ArgumentNullException("comparison");

            _compareFunction = comparison;
        }

        public override int Compare(T arg1, T arg2)
        {
            return _compareFunction(arg1, arg2);
        }
    }
}
