using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels.MessageModels
{
    public class MessageUserReceiveModel
    {
        public int UserId { get; set; }

        public int ChatId { get; set; }

        public string Message { get; set; }
    }
}
