using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels.UserModels
{
    public class UserListResponseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] Picture { get; set; }
    }
}
