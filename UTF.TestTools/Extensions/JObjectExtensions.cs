using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UTF.TestTools.Extensions
{
    public static class JObjectExtensions
    {
        public static IEnumerable<JToken> AllChildren(this JObject myJObject)
        {
            foreach (var c in myJObject.Children())
            {
                yield return c;
                //foreach (var cc in AllChildren(c.ToObject<JObject>()))
                //{
                //    yield return cc;
                //}
            }
        }
    }
}
