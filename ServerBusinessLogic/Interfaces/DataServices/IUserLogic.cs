using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
