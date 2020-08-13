using ChatTCPServer.Interfaces;
using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
{
    public class MessageLogic : IMessageLogic
    {
        public void AddMessage(Message message)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                context.Messages.Add(message);
                context.SaveChangesAsync();
            }
        }

        public void DeleteMessage(Message message)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                Message mess = context.Messages.FirstOrDefault(m => m.Id == message.Id);
                if (mess == null)
                    throw new Exception("Сообщение не найдено");
                context.Messages.Remove(mess);
                context.SaveChangesAsync();
            }
        }

        //TODO : pagination
        public List<Message> Read(Chat chat)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                return context.Messages.Where(m => chat == null || m.ChatId == chat.Id).ToList();
            }
        }
    }
}
