using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Interfaces
{
    public interface IRelatChatUserLogic
    {
        void AddUserToChat(User user, Chat chat);
        void RemoveUserFromChat(User user, Chat chat);
    }
}
