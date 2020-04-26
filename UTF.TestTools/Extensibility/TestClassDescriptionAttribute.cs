using System;
using System.Reflection;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TestClassDescriptionAttribute
        : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string _decription;

        // This is a positional argument
        public TestClassDescriptionAttribute(string decription = "")
        {
            this._decription = decription;
        }

        public string Description
        {
            get { return _decription; }
        }

        public override object TypeId
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }//  "UTF.TestTools.TestClassDescriptionAttribute"; }
        }
    }
}
