using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Interfaces
{
    public interface IUserLogic
    {
        void Create(UserReceiveModel user);
        void Update(UserReceiveModel user);
        void Delete(UserReceiveModel user);
        List<UserResponseModel> Read(UserReceiveModel user);
    }
}
