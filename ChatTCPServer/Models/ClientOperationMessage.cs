using ServerBusinessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Models
{
    public class ClientOperationMessage
    {
        public ClientOperations Operation { get; set; }
        public object Data { get; set; }
    }
}
