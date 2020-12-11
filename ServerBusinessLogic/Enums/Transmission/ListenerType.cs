using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Enums.Transmission
{
    public enum ListenerType
    {
        AuthorizationListener,
        RegistrationListener,

        ChatListListener,
        ChatListDeleteListener,
        ChatsMessagesListener,

        UserListListener
    }
}
