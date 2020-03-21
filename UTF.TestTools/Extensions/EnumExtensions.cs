using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace UTF.TestTools.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum obj)
        {
            FieldInfo fi = obj.GetType().GetField(obj.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return obj.ToString();
        }

        public static IEnumerable<Enum> GetFlags(this Enum obj)
        {
            return GetFlags(obj, Enum.GetValues(obj.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum obj)
        {
            return GetFlags(obj, GetFlagValues(obj.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum obj, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(obj);
            List<Enum> results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(obj) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(obj) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }
    }
}
