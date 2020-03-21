using System;
using System.Collections.Generic;

namespace UTF.TestTools.Converters
{
    internal class JsonClassEqualityComparer<T>
        : IEqualityComparer<T>
    {
        public bool Equals(T b1, T b2)
        {
            if (b2 == null && b1 == null)
                return true;
            else if (b1 == null | b2 == null)
                return false;
            else if (b1.GetHashCode() == b2.GetHashCode())
                return true;
            else
                return false;
        }

        public int GetHashCode(T bx)
        {
            return bx.GetHashCode();
        }
    }
}
