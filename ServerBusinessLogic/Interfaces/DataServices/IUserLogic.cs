using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System.Collections.Generic;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IUserLogic
    {
        void Create(UserReceiveModel user);

        void Update(UserReceiveModel user);

        void Delete(UserReceiveModel user);

        List<UserListResponseModel> ReadPage(UserPaginationReceiveModel user);

        UserResponseModel GetUser(UserReceiveModel user);
    }
}
