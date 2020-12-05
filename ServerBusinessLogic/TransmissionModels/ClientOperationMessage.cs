using ServerBusinessLogic.Enums.Transmission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.TransmissionModels
{
    public class ClientOperationMessage
    {
        public ClientOperations Operation { get; set; }
        public string JsonData { get; set; }
    }
}
