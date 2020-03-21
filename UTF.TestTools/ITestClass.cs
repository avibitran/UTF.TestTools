using System;
using System.Collections.Generic;
using System.Reflection;

namespace UTF.TestTools
{
    public interface ITestClass
    {
        string GetClassDescription();
        //string GetClassInfo();
        TestInfo GetTestInfo(string testId = null, int iteration = 0, string assemblyName = null, bool innerCall = false);
        string GetTestDescription(string testName = null, int iteration = 0);
        string GetAssemblyName();
        List<MethodInfo> GetTests(Type testClass = null);
    }
}
