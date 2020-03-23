using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.DAL
{
    // Insperation Post:  http://stackoverflow.com/questions/28674167/how-can-i-un-jsonignore-an-attribute-in-a-derived-class

    public class BaseTypeFilterContractResolver<T>
        : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(T).BaseType)
            {
                property.ShouldSerialize = instance => false;
            }
            return property;
        }
    }

    //public class BaseTypeFilterContractResolver
    //    : DefaultContractResolver
    //{
    //    public new static readonly BaseTypeFilterContractResolver Instance = new BaseTypeFilterContractResolver();
    //    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    //    {
    //        JsonProperty property = base.CreateProperty(member, memberSerialization);

    //        if ((property.DeclaringType.Equals(typeof(OpenProfileRequest).BaseType))  || 
    //            (property.DeclaringType.Equals(typeof(QueryExcuteRequest).BaseType))  ||
    //            (property.DeclaringType.Equals(typeof(CloseProfileRequest).BaseType)) ||
    //            (property.DeclaringType.Equals(typeof(AddInsightRequest).BaseType)))
    //        {
    //            property.ShouldSerialize = instance => false;
    //        }

    //        return property;
    //    }
    //}

}
    
    
    

