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
    public class MessageLogic : IMessageLogic
    {
        public void AddMessage(MessageReceiveModel messageModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                context.Messages.Add(new Message()
                {
                    Id = messageModel.Id,
                    ChatId = messageModel.ChatId,
                    FromUserId = messageModel.UserId,
                    UserMessage = messageModel.UserMassage
                });
                context.SaveChanges();
            }
        }

        public void DeleteMessage(MessageReceiveModel messageModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Message mess = context.Messages.FirstOrDefault(m => m.Id == messageModel.Id);
                if (mess == null)
                    throw new Exception("Сообщение не найдено");
                context.Messages.Remove(mess);
                context.SaveChanges();
            }
        }

        //TODO : pagination
        public List<MessageResponseModel> Read(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                return context.Messages.Where(m => chatModel == null || m.ChatId == chatModel.Id)
                    .Select(c => new MessageResponseModel()
                    {
                        Id = c.Id,
                        ChatId = c.ChatId,
                        UserId = c.FromUserId,
                        UserMassage = c.UserMessage
                    })
                    .ToList();
            }
        }
    }
}
