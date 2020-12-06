using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
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
    }
}
