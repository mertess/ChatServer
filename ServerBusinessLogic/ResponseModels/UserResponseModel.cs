using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Password { get; set; }
    }
}
