using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IChatLogic
    {
        void Create(ChatReceiveModel chat);

        void Update(ChatReceiveModel chat);

        void Delete(ChatReceiveModel chat);

        List<ChatResponseModel> Read(UserPaginationReceiveModel userPagination);

        List<UserListResponseModel> GetChatUsers(int chatId);
    }
}
