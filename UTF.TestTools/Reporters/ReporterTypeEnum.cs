using System;

namespace UTF.TestTools.Reporters
{
    [Flags]
    public enum ReporterTypeEnum
    {
        /// <summary>
        /// The default reporter. 
        ///     this means writing to the inner logger (xml file format).
        /// </summary>
        Default = 0,

        /// <summary>
        /// The Console reporter.
        ///     Writes formatted info to the console.
        /// </summary>
        ConsoleReporter = 1 << 0,

        /// <summary>
        /// The Html reporter.
        ///     generates an HTML report.
        /// </summary>
        HtmlReporter = 1 << 1,
    }
}
