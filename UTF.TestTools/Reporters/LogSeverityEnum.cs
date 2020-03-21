using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace UTF.TestTools.Reporters
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogSeverityEnum
    {
        [EnumMember(Value = "DebugVerbose")]
        DebugVerbose = 0,

        [EnumMember(Value = "Debug")]
        Debug = 1,

        [EnumMember(Value = "Verbose")]
        Verbose = 2,
        
        [EnumMember(Value = "Info")]
        Informational = 3,

        [EnumMember(Value = "DebugError")]
        DebugError = 4,

        [EnumMember(Value = "NonCritical")]
        NonCritical = 5,

        [EnumMember(Value = "Critical")]
        Critical = 7,

        [EnumMember(Value = "None")]
        None = 8,

        [EnumMember(Value = "Unknown")]
        Unknown = 99
    }
}
