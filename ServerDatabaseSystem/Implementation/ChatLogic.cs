using ServerBusinessLogic.Interfaces;
using ServerDatabaseSystem.DbModels;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.Implementation
{
    public class ChatLogic : IChatLogic
    {
        public void Create(ChatReceiveModel chatModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            { 
                if (context.Chats.FirstOrDefault(c => c.ChatName.Equals(chatModel.ChatName)) != null)
                    throw new Exception("Чат с таким названием уже есть!");
                context.Chats.Add(new Chat() { ChatName = chatModel.ChatName, CountUsers = 1 });
                context.SaveChanges();
            }
        }

        public void Delete(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id);
                if (cht == null)
                    throw new Exception("Чата с таким идентификатором нет в БД");
                context.Chats.Remove(cht);
                context.SaveChanges();
            }
        }

        public List<ChatResponseModel> Read(ChatReceiveModel chatModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                return context.Chats.Where(c => chatModel == null || c.ChatName.Equals(chatModel.ChatName)).Select(c =>
                new ChatResponseModel()
                {
                    Id = c.Id,
                    ChatName = c.ChatName,
                    CountUsers = c.CountUsers,
                    OwnerId = c.CreatorId
                }).ToList();
            }
        }

        public void Update(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id);
                if (cht == null)
                    throw new Exception("Чата с таким идентификатором нет в БД");
                if (context.Chats.FirstOrDefault(c => c.ChatName.Equals(chatModel.ChatName)) != null)
                    throw new Exception("Чат с таким названием уже есть в БД");
                cht.ChatName = chatModel.ChatName;
                cht.CountUsers = chatModel.CountUsers;
                context.SaveChanges();
            }
        }
    }
}
