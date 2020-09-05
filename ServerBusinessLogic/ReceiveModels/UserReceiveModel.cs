using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels
{
    public class UserReceiveModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Password { get; set; }
    }
}
