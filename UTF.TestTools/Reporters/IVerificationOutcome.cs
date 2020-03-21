using UTF.TestTools.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Reporters
{
    public interface IVerificationOutcome
    {
        StepStatusEnum Outcome { get; set; }
    }
}
