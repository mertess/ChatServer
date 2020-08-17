using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Models
{
    public class OperationResultInfo
    {
        public string Info { get; set; }
        public OperationsResults OperationResult { get; set; }

        public override string ToString()
        {
            return Enum.GetName(typeof(OperationsResults), OperationResult) + ":" + Info;
        }
    }
}
