using ChatTCPServer.Interfaces;
using ChatTCPServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
{
    public class ChatLogic : IChatLogic
    {
        public void Create(Chat chat)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            { 
                if (context.Chats.FirstOrDefault(c => c.ChatName.Equals(chat.ChatName)) != null)
                    throw new Exception("Чат с таким названием уже есть!");
                context.Chats.Add(new Chat() { ChatName = chat.ChatName, CountUsers = 1 });
                context.SaveChangesAsync();
            }
        }

        public void Delete(Chat chat)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                Chat cht = context.Chats.FirstOrDefault(c => c.ChatName.Equals(chat.ChatName));
                if (cht == null)
                    throw new Exception("Чата с таким названием нет в БД");
                context.Chats.Remove(cht);
                context.SaveChangesAsync();
            }
        }

        public List<Chat> Read(Chat chat)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                return context.Chats.Where(c => chat == null || c.ChatName.Equals(chat.ChatName)).ToList();
            }
        }

        public void Update(Chat chat)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                Chat cht = context.Chats.FirstOrDefault(c => c.ChatName.Equals(chat.ChatName));
                if (cht == null)
                    throw new Exception("Чата с таким названием нет в БД");
                cht.ChatName = chat.ChatName;
                cht.CountUsers = chat.CountUsers;
                context.SaveChangesAsync();
            }
        }
    }
}
