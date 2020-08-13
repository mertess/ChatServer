using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Interfaces
{
    public interface IMessageLogic
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        List<Message> Read(Chat chat);
    }
}
