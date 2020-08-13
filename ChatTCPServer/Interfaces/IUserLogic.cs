using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Interfaces
{
    public interface IUserLogic
    {
        void Create(User user);
        void Update(User user);
        void Delete(User user);
        List<User> Read(User user);
    }
}
