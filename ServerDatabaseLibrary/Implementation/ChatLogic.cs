﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.Models;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.ChatModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerDatabaseSystem.DbModels;
using ServerDatabaseSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace ServerDatabaseSystem.Implementation
{
    /// <summary>
    /// Service for working with Chats database table 
    /// </summary>
    public class ChatLogic : IChatLogic
    {
        /// <summary>
        /// <see cref="UserChatBinder"/>
        /// </summary>
        private readonly UserChatBinder _userChatBinder;

        private readonly MessageLogic _messageLogic;

        public ChatLogic() 
        { 
            _userChatBinder = new UserChatBinder();
            _messageLogic = new MessageLogic();
        }

        /// <summary>
        /// Creating a new chat in Database and binding users with that chat  
        /// </summary>
        /// <param name="chatModel"><see cref="ChatReceiveModel"/></param>
        /// <returns><see cref="ChatResponseModel"/></returns>
        public ChatResponseModel Create(ChatReceiveModel chatModel)
        {
            //TODO : review
            using (DatabaseContext context = new DatabaseContext())
            {
                if (string.IsNullOrEmpty(chatModel.ChatName) && chatModel.ChatUsers.Count() > 2)
                    throw new Exception("Ошибка создания чата. Чат в котором больше 2ух людей должен иметь название");

                context.Chats.Add(new Chat()
                {
                    ChatName = chatModel.ChatName,
                    CreatorId = chatModel.CreatorId,
                    DateOfCreation = chatModel.DateOfCreation,
                    IsPrivate = chatModel.ChatUsers.Count() == 2 ? true : false,
                    CountUsers = chatModel.ChatUsers.Count()
                });
                context.SaveChanges();

                //getting a added chat 
                var addedChat = context.Chats.FirstOrDefault(c => c.CreatorId == chatModel.CreatorId && c.DateOfCreation.Equals(chatModel.DateOfCreation));

                //binding received users id with new chat id
                chatModel.ChatUsers.ForEach(cu => cu.ChatId = addedChat.Id);

                //binding users with chat 
                _userChatBinder.AddUsersToChat(chatModel.ChatUsers, context);

                return new ChatResponseModel()
                {
                    Id = addedChat.Id,
                    CreatorId = addedChat.CreatorId,
                    ChatName = addedChat.ChatName,
                    CountUsers = addedChat.CountUsers,
                    ChatUsers = GetChatUsers(addedChat.Id)
                };
            }
        }

        /// <summary>
        /// Removing chat from Database and remove user bindings with that chat
        /// </summary>
        /// <param name="chatModel"><see cref="ChatReceiveModel"/></param>
        public void Delete(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!chatModel.Id.HasValue)
                            throw new Exception("Ошибка передачи данных, свойство Id модели не было установлено");

                        Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id.Value);
                        if (cht == null)
                            throw new Exception("Чата с таким идентификатором нет в БД");

                        //remove binded users 
                        _userChatBinder.RemoveUsersFromChat(chatModel.ChatUsers, context);

                        context.Chats.Remove(cht);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Getting chats by UserId from database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ChatResponseModel> GetChatsByUserId(int userId)
        {
            using (var context = new DatabaseContext())
            {
                return context.RelationChatUsers
                    .Where(rcu => rcu.UserId == userId)
                    .Include(rcu => rcu.Chat)
                    .ToList()
                    .Select(rcu => new ChatResponseModel()
                    {
                        Id = rcu.ChatId,
                        ChatName = rcu.Chat.ChatName,
                        CreatorId = rcu.Chat.CreatorId,
                        CountUsers = rcu.Chat.CountUsers,
                        ChatUsers = GetChatUsers(rcu.ChatId)
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Getting chat and binded users with that chat using chat Id 
        /// </summary>
        /// <param name="model"><see cref="ChatReceiveModel"/></param>
        /// <returns><see cref="ChatResponseModel"/></returns>
        public ChatResponseModel GetChat(ChatReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                var chat = context.Chats.FirstOrDefault(c => c.Id == model.Id);
                if (chat == null)
                    throw new Exception("Чат не найден в БД");

                return new ChatResponseModel()
                {
                    Id = chat.Id,
                    CreatorId = chat.CreatorId,
                    ChatName = chat.ChatName,
                    CountUsers = chat.CountUsers,
                    ChatUsers = GetChatUsers(chat.Id)
                };
            }
        }

        /// <summary>
        /// Getting user from chat using chat Id
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns><see cref="UserListResponseModel"/></returns>
        public List<ChatUserResponseModel> GetChatUsers(int chatId)
        {
            using (var context = new DatabaseContext())
            {
                return context.RelationChatUsers
                    .Where(rcu => rcu.ChatId == chatId)
                    .Include(rcu => rcu.User)
                    .Select(rcu => new ChatUserResponseModel()
                    {
                        Id = rcu.UserId,
                        UserName = rcu.User.UserName,
                        Picture = new FileModel()
                        {
                            FileName = rcu.User.PictureName,
                            Extension = rcu.User.PictureExtension,
                            BinaryForm = rcu.User.Picture
                        },
                        IsOnline = rcu.User.IsOnline
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Get page of chats of user by Id 
        /// </summary>
        /// <param name="userPagination"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns>List of ChatResponceModel <see cref="ChatResponseModel"/></returns>
        public List<ChatResponseModel> ReadPage(UserPaginationReceiveModel userPagination)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var relationChatUsers = context.RelationChatUsers
                    .Where(rcu => rcu.UserId == userPagination.UserId)
                    .Include(rcu => rcu.Chat)
                    .Select(rcu => rcu.Chat);

                relationChatUsers.Reverse();

                return relationChatUsers
                    .Skip(userPagination.Page * 10)
                    .Take(10)
                    .Select(chat => new ChatResponseModel()
                    {
                        Id = chat.Id,
                        ChatName = chat.ChatName,
                        CreatorId = chat.CreatorId,
                        ChatUsers = context.RelationChatUsers
                            .Where(rcu => rcu.ChatId == chat.Id)
                            .Include(rcu => rcu.User)
                            .Select(rcu => new ChatUserResponseModel()
                            {
                                Id = rcu.User.Id,
                                UserName = rcu.User.UserName,
                                Picture = new FileModel() 
                                {
                                    FileName = rcu.User.PictureName,
                                    BinaryForm = rcu.User.Picture,
                                    Extension = rcu.User.PictureExtension
                                },
                                IsOnline = rcu.User.IsOnline
                            })
                            .ToList(),
                        CountUsers = chat.CountUsers,
                        LastMessages = _messageLogic.ReadPage(new ChatPaginationReceiveModel() { ChatId = chat.Id, Page = 0 })
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating chat in database, rebinding users 
        /// </summary>
        /// <param name="chatModel"><see cref="ChatReceiveModel"/></param>
        public ChatResponseModel Update(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!chatModel.Id.HasValue)
                            throw new Exception("Ошибка передачи данных, свойство Id модели не было установлено");

                        Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id.Value);

                        if (cht == null)
                            throw new Exception("Чата с таким идентификатором нет в БД");

                        if (cht.CountUsers > 2)
                        {
                            cht.ChatName = chatModel.ChatName ?? cht.ChatName;
                            context.SaveChanges();
                        }

                        if (chatModel.ChatUsers != null)
                        {
                            if (cht.CountUsers == 2 && chatModel.ChatUsers.Count() > 2)
                                return Create(chatModel);
                            else
                            {
                                cht.CountUsers = chatModel.ChatUsers.Count();
                                cht.IsPrivate = chatModel.ChatUsers.Count() == 2 ? true : false;
                                context.SaveChanges();

                                var chatUsersFromDb = context.RelationChatUsers.Where(rcu => rcu.ChatId == cht.Id).ToList();

                                var addBindings = chatModel.ChatUsers
                                    .Where(cu => chatUsersFromDb.FirstOrDefault(rcu => rcu.UserId == cu.UserId) == null)
                                    .ToList();

                                //adding users that isn't contains in database
                                _userChatBinder.AddUsersToChat(addBindings, context);
                                context.SaveChanges();

                                //removing users that isn't contains in chatModel
                                var removeBindings = chatUsersFromDb
                                    .Where(rcu => chatModel.ChatUsers.FirstOrDefault(cu => cu.UserId == rcu.UserId) == null)
                                    .Select(rcu => new ChatUserReceiveModel()
                                    {
                                        UserId = rcu.UserId,
                                        ChatId = rcu.ChatId
                                    })
                                    .ToList();

                                _userChatBinder.RemoveUsersFromChat(removeBindings, context);
                                context.SaveChanges();
                            }
                        }

                        transaction.Commit();

                        //getting its updated chat from database
                        var updatedChat = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id.Value);

                        return new ChatResponseModel()
                        {
                            Id = updatedChat.Id,
                            CreatorId = updatedChat.CreatorId,
                            ChatName = updatedChat.ChatName,
                            CountUsers = updatedChat.CountUsers,
                            ChatUsers = GetChatUsers(updatedChat.Id)
                        };
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
