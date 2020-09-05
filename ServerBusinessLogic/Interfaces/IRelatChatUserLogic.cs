using ServerBusinessLogic.ReceiveModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Interfaces
{
    public interface IRelatChatUserLogic
    {
        void AddUserToChat(UserReceiveModel user, ChatReceiveModel chat);
        void RemoveUserFromChat(UserReceiveModel user, ChatReceiveModel chat);
    }
}
