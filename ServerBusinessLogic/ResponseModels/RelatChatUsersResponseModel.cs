using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels
{
    public class RelatChatUsersResponseModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
