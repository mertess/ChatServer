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
        ChatResponseModel Create(ChatReceiveModel chat);

        void Update(ChatReceiveModel chat);

        void Delete(ChatReceiveModel chat);

        List<ChatResponseModel> ReadPage(UserPaginationReceiveModel userPagination);

        List<ChatUserResponseModel> GetChatUsers(int chatId);

        ChatResponseModel GetChat(ChatReceiveModel model);
    }
}
