using ChatTCPServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Interfaces
{
    public interface IChatLogic
    {
        void Create(Chat chat);
        void Update(Chat chat);
        void Delete(Chat chat);
        List<Chat> Read(Chat chat);
    }
}
