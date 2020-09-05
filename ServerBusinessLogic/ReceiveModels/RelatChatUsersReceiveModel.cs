using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels
{
    public class RelatChatUsersReceiveModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
