using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Serialization;
using UTF.TestTools.Converters;

namespace UTF.TestTools.Reporters
{
    //[JsonObject(Id = "verification")]
    //[XmlType(TypeName = "verification")]
    //[Serializable]
    //public class VerificationPoint
    //    : IVerificationOutcome
    //{
    //    #region Fields
    //    private StepStatusEnum _status = StepStatusEnum.Done;
    //    private string _expected = null;
    //    private string _actual = null;
    //    private List<string> _messages = null;
    //    #endregion Fields

    //    #region Ctor
    //    public VerificationPoint()
    //    { }

    //    public VerificationPoint(string expected, string actual = null, StepStatusEnum status = StepStatusEnum.Done, params string[] messages)
    //    {
    //        _expected = expected;
    //        _actual = actual;
    //        _status = status;

    //        if(messages.Length > 0)
    //        {
    //            _messages = new List<string>(messages);
    //        }

    //    }
    //    #endregion Ctor

    //    #region Methods
    //    public void AddMessage(string message)
    //    {
    //        this.Messages.Add(message);
    //    }

    //    #region Private Methods
    //    protected virtual void OnStatusChanged(IVerificationOutcome verification)
    //    {
    //        EventHandler<StatusChangedEventArgs> handler = StatusChanged;

    //        if (handler != null)
    //        {
    //            handler.Invoke(this, new StatusChangedEventArgs() { Verification = this });
    //        }
    //    }
    //    #endregion Private Methods
    //    #endregion Methods

    //    #region Properties
    //    /// <summary>
    //    /// Holds the status of the test.
    //    /// <para/> Order = 0
    //    /// </summary>
    //    [JsonProperty("status")]
    //    [XmlAttribute(AttributeName = "status")]
    //    public StepStatusEnum Outcome
    //    {
    //        get { return _status; }
    //        set
    //        {
    //            if (_status != value)
    //            {
    //                _status = value;
    //                OnStatusChanged(this);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Holds the step expected result.
    //    /// <para/> Order = 1
    //    /// </summary>
    //    [JsonProperty("expected")]
    //    [XmlIgnore]
    //    public string Expected
    //    {
    //        get { return _expected; }
    //        set { _expected = value; }
    //    }

    //    /// <summary>
    //    /// Holds the step expected result.
    //    /// <para/> Order = 1
    //    /// </summary>
    //    [JsonIgnore]
    //    [XmlElement(ElementName = "exp")]
    //    public System.Xml.XmlCDataSection ExpectedCData
    //    {
    //        get
    //        {
    //            if (String.IsNullOrEmpty(_expected))
    //                return new System.Xml.XmlDocument().CreateCDataSection("");
    //            else
    //                return new System.Xml.XmlDocument().CreateCDataSection(_expected);
    //        }
    //        set { _expected = value.Value.Replace("&bull; ", ""); }
    //    }

    //    /// <summary>
    //    /// Holds the step actual result.
    //    /// <para/> Order = 2
    //    /// </summary>
    //    [JsonProperty("actual")]
    //    [JsonConverter(typeof(ArrayJsonConverter<string>))]
    //    [XmlIgnore]
    //    public string Actual
    //    {
    //        get { return _actual; }
    //        set { _actual = value; }
    //    }

    //    /// <summary>
    //    /// Holds the step actual result.
    //    /// <para/> Order = 2
    //    /// </summary>
    //    [JsonIgnore]
    //    [XmlElement(ElementName = "act")]
    //    public System.Xml.XmlCDataSection ActualCData
    //    {
    //        get
    //        {
    //            if (String.IsNullOrEmpty(_actual))
    //                return new System.Xml.XmlDocument().CreateCDataSection("");
    //            else
    //                return new System.Xml.XmlDocument().CreateCDataSection(_actual);
    //        }
    //        set { _actual = value.Value.Replace("&bull; ", ""); }
    //    }

    //    /// <summary>
    //    /// Holds the step's messages.
    //    /// <para/> Order = 3
    //    /// </summary>
    //    [JsonProperty("messages")]
    //    [Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<string>))]
    //    [XmlIgnore]
    //    public List<string> Messages
    //    {
    //        get
    //        {
    //            if (_messages == null)
    //                _messages = new List<string>();

    //            return _messages;
    //        }
    //        set { _messages = value; }
    //    }

    //    /// <summary>
    //    /// Holds the step's messages.
    //    /// <para/> Order = 3
    //    /// </summary>
    //    [JsonIgnore]
    //    [XmlElement(ElementName = "msgs")]
    //    public System.Xml.XmlCDataSection MessagesCData
    //    {
    //        get
    //        {
    //            StringBuilder value = new StringBuilder("");

    //            if (_messages == null)
    //                return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
    //            else
    //            {
    //                foreach (string line in _messages)
    //                    value.AppendFormat("{0} {1}{2}", "&bull; ", line, "<br>");

    //                return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
    //            }
    //        }
    //        set
    //        {
    //            _messages = new List<string>();
    //            string[] lines = value.Value.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

    //            foreach (string line in lines)
    //                _messages.Add(line.Replace("&bull; ", ""));
    //        }
    //    }
    //    #endregion Properties

    //    #region Events
    //    public event EventHandler<StatusChangedEventArgs> StatusChanged;
    //    #endregion Events
    //}
}
