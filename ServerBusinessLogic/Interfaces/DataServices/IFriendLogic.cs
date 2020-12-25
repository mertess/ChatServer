using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System.Collections.Generic;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IFriendLogic
    {
        void Create(FriendReceiveModel model);

        void Delete(FriendReceiveModel model);

        List<UserListResponseModel> ReadPage(UserPaginationReceiveModel model);

        List<UserListResponseModel> ReadFriends(int userId);
    }
}
