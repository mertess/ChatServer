using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Enums.Transmission
{
    public enum ClientOperations
    {
        Authorization,
        Registration,
        UpdateProfile,
        GetUsers,

        SendMessage,
        CreateChat,
        DeleteChat,
        UpdateChat,
        GetChats
    }
}
