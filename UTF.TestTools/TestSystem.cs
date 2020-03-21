using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UTF.TestTools.Converters;

namespace UTF.TestTools
{
    [Flags]
    //[JsonConverter(typeof(FlagsEnumConverter))]
    public enum TestMachineTypeEnum
    {
        None = 0,
        SA = 1,
        Master = 2,
        Recorder = 4,
        Hdr = 8,
        MultiSite = 16,
    }

    public enum MatchTypeEnum
    {
        Exact,
        Contains,
    }

    [JsonObject("system")]
    public class TestSystem
    {
        #region Fields
        private string _name = String.Empty;
        private long _id = long.MinValue;
        private List<TestMachine> _machines = new List<TestMachine>();
        #endregion Fields

        #region Ctor
        public TestSystem()
        { }
        #endregion Ctor

        #region Methods
        public List<TestMachine> GetMachinesContainsType(TestMachineTypeEnum types = TestMachineTypeEnum.None)
        {
            if ((int)types == (int)TestMachineTypeEnum.None)
                return this.Machines;

            List<TestMachine> machines = new List<TestMachine>();

            foreach (TestMachine machine in this.Machines)
            {
                if ((machine.MachineTypes & types) > 0)
                {
                    machines.Add(machine);
                }
            }

            return machines;
        }

        public List<TestMachine> GetMachinesByType(TestMachineTypeEnum types = TestMachineTypeEnum.None)
        {
            if ((int)types == (int)TestMachineTypeEnum.None)
                return this.Machines;

            List<TestMachine> machines = new List<TestMachine>();

            foreach (TestMachine machine in this.Machines)
            {
                if ((machine.MachineTypes ^ types) == 0)
                {
                    machines.Add(machine);
                }
            }

            return machines;
        }

        public TestMachine GetMachineByIndex(TestMachineTypeEnum type, int index)
        {
            List<TestMachine> machines = null;

            machines = GetMachinesByType(type);

            if ((machines.Count == 0) || (machines.Count <= index))
                throw new IndexOutOfRangeException(String.Format("the index: {0}, is out of range: 0-{1}", index, (machines.Count - 1)));

            return machines[index];
        }

        public TestMachine GetMachineById(int id)
        {
            return _machines.Find(i => i.Id == id);
        }

        public List<string> GetMachinesHosts(TestMachineTypeEnum type)
        {
            List<string> hosts = null;

            List<TestMachine> list = GetMachinesByType(type);
            foreach (TestMachine machine in list)
            {
                if (hosts == null)
                    hosts = new List<string>();

                hosts.Add(machine.Host);
            }

            return hosts;
        }

        public List<TestMachine> GetRecorders()
        {
            return GetMachinesByType(TestMachineTypeEnum.Recorder);
        }

        public TestMachine GetRecorder(int index)
        {
            return GetMachineByIndex(TestMachineTypeEnum.Recorder, index);
        }

        #region Private Methods
        public static List<TestMachine> FilterMachinesByType(List<TestMachine> machines, TestMachineTypeEnum machineType)
        {
            return machines.FindAll(i => (i.MachineTypes & machineType) > 0);
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        [JsonProperty(PropertyName = "name", Order = 0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [JsonProperty(PropertyName = "id", Order = 1)]
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<TestMachine>))]
        [JsonProperty(PropertyName = "machines")]
        public List<TestMachine> Machines
        {
            get { return _machines; }
            set
            {
                if (value == null)
                    _machines = new List<TestMachine>();
                else
                    _machines = value;
            }
        }
        #endregion Properties
    }

    [JsonObject("machine")]
    public class TestMachine
    {
        #region Fields
        private string _host = String.Empty;
        private long _id = long.MinValue;
        private TestMachineTypeEnum _machineType = TestMachineTypeEnum.None;
        private string _description = String.Empty;
        private long _controller = -1;
        private long _systemId = -1;
        #endregion Fields

        #region Ctor
        public TestMachine()
        { }
        #endregion Ctor

        #region Methods
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        #endregion Methods

        #region Properties
        [JsonProperty(PropertyName = "systemId")]
        public long SystemId
        {
            get { return _systemId; }
            set { _systemId = value; }
        }

        [JsonProperty(PropertyName = "id")]
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(FlagsEnumConverter))]
        public TestMachineTypeEnum MachineTypes
        {
            get { return _machineType; }
            set { _machineType = value; }
        }

        [JsonProperty(PropertyName = "host")]
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return _description; }
            set { _description = value ?? String.Empty; }
        }

        [JsonProperty(PropertyName = "controller")]
        public long Controller
        {
            get { return _controller; }
            set { _controller = (value < 0) ? -1 : value; }
        }
        #endregion Properties
    }
}
