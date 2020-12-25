using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.Models;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <returns>?<see cref="MessageResponseModel"/></returns>
        public MessageResponseModel AddMessage(MessageReceiveModel messageModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                context.Messages.Add(new Message()
                {
                    ChatId = messageModel.ChatId,
                    FromUserId = messageModel.FromUserId,
                    Date = messageModel.Date,
                    UserMessage = messageModel.UserMassage,
                    FileName = messageModel.File?.FileName,
                    FileExtension = messageModel.File?.Extension,
                    File = messageModel.File?.BinaryForm
                });
                context.SaveChanges();

                //getting added message from database 
                var addedMessage = context.Messages.FirstOrDefault(m => m.ChatId == messageModel.ChatId
                && m.FromUserId == messageModel.FromUserId
                && m.UserMessage.Equals(messageModel.UserMassage)
                && m.Date.Equals(messageModel.Date));

                return new MessageResponseModel()
                {
                    Id = addedMessage.Id,
                    UserId = addedMessage.FromUserId,
                    ChatId = addedMessage.ChatId,
                    Date = addedMessage.Date,
                    UserMassage = addedMessage.UserMessage,
                    File = string.IsNullOrEmpty(addedMessage.FileName) ? null : new FileModel()
                    {
                        FileName = addedMessage.FileName,
                        Extension = addedMessage.FileExtension,
                        BinaryForm = addedMessage.File
                    }
                };
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
                        UserId = m.FromUserId,
                        Date = m.Date,
                        File = string.IsNullOrEmpty(m.FileName) ? null : new FileModel()
                        {
                            FileName = m.FileName,
                            Extension = m.FileExtension,
                            BinaryForm = m.File
                        }
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating exist message in Messages database table
        /// </summary>
        /// <param name="message"><see cref="MessageReceiveModel"/></param>
        public MessageResponseModel Update(MessageReceiveModel message)
        {
            using (var context = new DatabaseContext())
            {
                if (message.Id.HasValue)
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

                //getting updated message from database 
                var updatedMessage = context.Messages.FirstOrDefault(m => m.Id == message.Id);

                return new MessageResponseModel()
                {
                    Id = updatedMessage.Id,
                    UserId = updatedMessage.FromUserId,
                    ChatId = updatedMessage.ChatId,
                    Date = updatedMessage.Date,
                    UserMassage = updatedMessage.UserMessage,
                    File = string.IsNullOrEmpty(updatedMessage.FileName) ? null : new FileModel()
                    {
                        FileName = updatedMessage.FileName,
                        Extension = updatedMessage.FileExtension,
                        BinaryForm = updatedMessage.File
                    }
                };
            }
        }
    }
}
