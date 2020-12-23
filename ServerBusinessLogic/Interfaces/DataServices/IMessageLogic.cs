using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using System.Collections.Generic;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IMessageLogic
    {
        MessageResponseModel AddMessage(MessageReceiveModel message);

        void DeleteMessage(MessageReceiveModel message);

        MessageResponseModel Update(MessageReceiveModel message);

        List<MessageResponseModel> ReadPage(ChatPaginationReceiveModel chat);
    }
}
