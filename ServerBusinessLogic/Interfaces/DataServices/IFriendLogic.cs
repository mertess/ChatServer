using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IFriendLogic
    {
        void CreateFriend();

        void DeleteFriend();

        void Read();
    }
}
