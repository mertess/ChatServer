using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using System.Collections.Generic;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IChatLogic
    {
        ChatResponseModel Create(ChatReceiveModel chat);

        ChatResponseModel Update(ChatReceiveModel chat);

        void Delete(ChatReceiveModel chat);

        List<ChatResponseModel> ReadPage(UserPaginationReceiveModel userPagination);

        List<ChatUserResponseModel> GetChatUsers(int chatId);

        ChatResponseModel GetChat(ChatReceiveModel model);

        List<ChatResponseModel> GetChatsByUsersId(List<int> usersId);
    }
}
