using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Interfaces
{
    public interface IMessageLogic
    {
        void AddMessage(MessageReceiveModel message);
        void DeleteMessage(MessageReceiveModel message);
        List<MessageResponseModel> Read(ChatReceiveModel chat);
    }
}
