using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels.MessageModels
{
    public class MessageReceiveModel
    {
        public int? Id { get; set; }

        public int FromUserId { get; set; }

        public int ChatId { get; set; }

        public DateTime Date { get; set; }

        public string UserMassage { get; set; }

        public bool IsReaded { get; set; }
    }
}
