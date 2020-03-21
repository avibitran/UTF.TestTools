using System;
using System.Xml.Serialization;

namespace UTF.TestTools.Reporters
{
    [Serializable]
    public enum StepStatusEnum
    {
        /// <summary>
        /// For steps that does not have verification.
        /// <para/> The status does not change the outcome of the test.
        /// <para/> Code: -1
        /// </summary>
        [XmlEnum("-1")]
        Done = -1,

        /// <summary>
        /// For steps that have a passed verification.
        /// <para/> The status does change the outcome of the test.
        /// <para/> Code: 0
        /// </summary>
        [XmlEnum("0")]
        Pass = 0,

        /// <summary>
        /// For steps that does not have verification, but running through the test found problems that should not fail the test.
        /// <para/> The status does change the outcome of the test.
        /// <para/> Code: 1
        /// </summary>
        [XmlEnum("1")]
        Warning = 1,

        /// <summary>
        /// For steps that have a failed verification.
        /// <para/> The status does change the outcome of the test.
        /// <para/> Code: 2
        /// </summary>
        [XmlEnum("2")]
        Fail = 2,

        /// <summary>
        /// For steps that have problem continuing running, due to bug in the system that was not found through verification.
        /// <para/> The status does change the outcome of the test.
        /// <para/> Code: 3
        /// </summary>
        [XmlEnum("3")]
        Error = 3,

        /// <summary>
        /// For steps that have thrown an exception in the execution.
        /// <para/> The status does change the outcome of the test.
        /// <para/> Code: 4
        /// </summary>
        [XmlEnum("4")]
        Fatal = 4,
    }
}
