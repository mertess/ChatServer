using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels.ChatModels
{
    public class ChatReceiveModel
    {
        public int? Id { get; set; }

        public string ChatName { get; set; }

        public int CreatorId { get; set; }

        public DateTime DateOfCreation { get; set; }

        public List<ChatUserReceiveModel> ChatUsers { get; set; }
    }
}
