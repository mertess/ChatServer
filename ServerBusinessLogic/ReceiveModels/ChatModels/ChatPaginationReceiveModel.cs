using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels.ChatModels
{
    public class ChatPaginationReceiveModel
    {
        public int ChatId { get; set; }

        public int Page { get; set; }
    }
}
