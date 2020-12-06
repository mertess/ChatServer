using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels
{
    public class ChatUserResponseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] Picture { get; set; }

        public bool IsOnline { get; set; }
    }
}
