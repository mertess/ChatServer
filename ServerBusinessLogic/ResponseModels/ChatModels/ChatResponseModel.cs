using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels.ChatModels
{
    public class ChatResponseModel
    {
        public int Id { get; set; }

        public string ChatName { get; set; }

        public int CreatorId { get; set; }

        public int CountUsers { get; set; }

        public List<ChatUserResponseModel> ChatUsers { get; set; }
    }
}
