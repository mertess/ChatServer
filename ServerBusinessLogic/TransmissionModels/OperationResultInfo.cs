using ServerBusinessLogic.Enums.Transmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.TransmissionModels
{
    public class OperationResultInfo
    {
        public ListenerType ToListener { get; set; }
        public string ErrorInfo { get; set; }
        public OperationsResults OperationResult { get; set; }
        public string JsonData { get; set; }
    }
}
