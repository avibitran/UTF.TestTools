using System;
using System.Collections.Generic;
using System.Reflection;

namespace UTF.TestTools
{
    public interface ITestClass
    {
        string GetClassDescription(Type testClass = null);
        //string GetClassInfo();
        object ExecuteTest(string testId, string assemblyName = null, params object[] args);
        string GetTestDescription(MethodInfo methodInfo, int iteration = 0);
        string GetAssemblyName();
        List<MethodInfo> GetTests(Type testClass = null);
    }
}
