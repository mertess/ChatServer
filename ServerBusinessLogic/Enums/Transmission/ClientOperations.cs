using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Enums.Transmission
{
    public enum ClientOperations
    {
        //users
        Authorization,
        Registration,
        UpdateProfile,
        GetUsers,

        //messages
        SendMessage,
        GetMessages,
        UpdateMessage,
        DeleteMessage,

        //chats
        CreateChat,
        DeleteChat,
        UpdateChat,
        GetChats,

        //friends
        AddFriend,
        DeleteFriend,
        GetFriends,

        //notifications
        UpdateNotification,
        GetNotifications 
    }
}
