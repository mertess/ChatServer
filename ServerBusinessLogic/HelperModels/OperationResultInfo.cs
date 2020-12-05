using ServerBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.HelperModels
{
    public class OperationResultInfo
    {
        public ListenerType ToListener { get; set; }
        public string ErrorInfo { get; set; }
        public OperationsResults OperationResult { get; set; }
        public string Data { get; set; }

        //TODO : review 
        public override string ToString()
        {
            return Enum.GetName(typeof(ListenerType), ToListener) + "," 
                + Enum.GetName(typeof(OperationsResults), OperationResult) + ":" + ErrorInfo + "," + Data;
        }
    }
}
