using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IFriendLogic
    {
        void Create(FriendReceiveModel model);

        void Delete(FriendReceiveModel model);

        List<UserListResponseModel> ReadPage(UserPaginationReceiveModel model);
    }
}
