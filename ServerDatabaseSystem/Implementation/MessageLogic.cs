using ServerBusinessLogic.Interfaces.DataServices;
using ServerDatabaseSystem.DbModels;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ResponseModels.MessageModels;

namespace ServerDatabaseSystem.Implementation
{
    /// <summary>
    /// Service for working with Messages database table 
    /// </summary>
    public class MessageLogic : IMessageLogic
    {
        /// <summary>
        /// Adding a new message to Messages database table
        /// </summary>
        /// <param name="messageModel"><see cref="MessageReceiveModel"/></param>
        public void AddMessage(MessageReceiveModel messageModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                context.Messages.Add(new Message()
                {
                    ChatId = messageModel.ChatId,
                    FromUserId = messageModel.FromUserId,
                    Date = messageModel.Date,
                    UserMessage = messageModel.UserMassage
                });
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting message from Messages database table by Id
        /// </summary>
        /// <param name="messageModel"><see cref="MessageReceiveModel"/></param>
        public void DeleteMessage(MessageReceiveModel messageModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                if (messageModel.Id.HasValue)
                {
                    var mess = context.Messages.FirstOrDefault(m => m.Id == messageModel.Id.Value);
                    if (mess == null)
                        throw new Exception("Сообщение не найдено");
                    context.Messages.Remove(mess);
                    context.SaveChanges();
                }
                else
                    throw new Exception("Ошибка передачи данных, у модели не установлено свойство Id");
            }
        }

        /// <summary>
        /// Reading page of messages of chat by Id
        /// </summary>
        /// <param name="chatModel"></param>
        /// <returns></returns>
        public List<MessageResponseModel> ReadPage(ChatPaginationReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                return context.Messages
                    .Where(m => m.ChatId == chatModel.ChatId)
                    .Skip(chatModel.Page * 10)
                    .Take(10)
                    .Select(m => new MessageResponseModel()
                    {
                        Id = m.Id,
                        UserMassage = m.UserMessage,
                        ChatId = m.ChatId,
                        UserId = m.FromUserId
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating exist message in Messages database table
        /// </summary>
        /// <param name="message"><see cref="MessageReceiveModel"/></param>
        public void Update(MessageReceiveModel message)
        {
            using (var context = new DatabaseContext())
            {
                if(message.Id.HasValue)
                {
                    var messageDb = context.Messages.FirstOrDefault(m => m.Id == message.Id.Value);
                    if (messageDb == null)
                        throw new Exception("Сообщение не найдено");

                    messageDb.UserMessage = message.UserMassage;
                    messageDb.IsReaded = message.IsReaded;
                    context.SaveChanges();
                }
                else
                    throw new Exception("Ошибка передачи данных, у модели не установлено свойство Id");
            }
        }
    }
}
