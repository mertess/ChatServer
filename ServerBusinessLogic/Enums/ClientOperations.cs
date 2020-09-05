using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Enums
{
    public enum ClientOperations
    {
        Authorization,
        Registration,
        UpdateProfile,
        SendMessage,

        CreateChat,
        DeleteChat,
        AddUserToChat,
        RemoveUserToChat
    }
}
