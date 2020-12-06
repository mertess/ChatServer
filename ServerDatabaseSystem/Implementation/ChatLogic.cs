using ServerBusinessLogic.Interfaces.DataServices;
using ServerDatabaseSystem.DbModels;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ServerDatabaseSystem.Services;

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

        public ChatLogic() => _userChatBinder = new UserChatBinder();

        /// <summary>
        /// Creating a new chat in Database and binding users with that chat  
        /// </summary>
        /// <param name="chatModel"><see cref="ChatReceiveModel"/></param>
        public void Create(ChatReceiveModel chatModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            { 
                if (context.Chats.FirstOrDefault(c => c.ChatName.Equals(chatModel.ChatName)) != null)
                    throw new Exception("Чат с таким названием уже есть!");
                context.Chats.Add(new Chat()
                { 
                    ChatName = chatModel.ChatName,
                    CreatorId = chatModel.CreatorId,
                    DateOfCreation = DateTime.Now,
                    IsPrivate = chatModel.ChatUsers.Count() == 2 ? true : false,
                    CountUsers = chatModel.ChatUsers.Count() 
                });
                context.SaveChanges();

                //binding users with chat 
                _userChatBinder.AddUsersToChat(chatModel.ChatUsers, context);
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
                        //remove binded users 
                        _userChatBinder.RemoveUsersFromChat(chatModel.ChatUsers, context);

                        Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id);
                        if (cht == null)
                            throw new Exception("Чата с таким идентификатором нет в БД");
                        context.Chats.Remove(cht);
                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Get page of chats of user by Id 
        /// </summary>
        /// <param name="userPagination"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns>List of ChatResponceModel <see cref="ChatResponseModel"/></returns>
        public List<ChatResponseModel> Read(UserPaginationReceiveModel userPagination)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                return context.Chats
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
                                Picture = rcu.User.Picture,
                                IsOnline = rcu.User.IsOnline
                            })
                            .ToList(),
                        CountUsers = chat.CountUsers
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating chat in database, rebinding users 
        /// </summary>
        /// <param name="chatModel"><see cref="ChatReceiveModel"/></param>
        public void Update(ChatReceiveModel chatModel)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Chat cht = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id);
                        if (cht == null)
                            throw new Exception("Чата с таким идентификатором нет в БД");
                        cht.ChatName = chatModel.ChatName;
                        cht.CountUsers = chatModel.ChatUsers.Count();
                        cht.IsPrivate = chatModel.ChatUsers.Count() == 2 ? true : false;
                        context.SaveChanges();

                        //removing users that isn't contains in chatModel
                        var removeBindings = context.RelationChatUsers
                            .Where(rcu => rcu.ChatId == cht.Id && chatModel.ChatUsers.FirstOrDefault(cu => cu.UserId == rcu.UserId) == null)
                            .Select(rcu => new ChatUserReceiveModel() 
                            {
                                UserId = rcu.UserId,
                                ChatId = rcu.ChatId
                            })
                            .ToList();

                        _userChatBinder.RemoveUsersFromChat(removeBindings, context);
                        context.SaveChanges();

                        var addBindings = chatModel.ChatUsers
                            .Where(cu => context.RelationChatUsers.FirstOrDefault(rcu => rcu.UserId == cu.UserId && rcu.ChatId == cu.ChatId) == null)
                            .ToList();

                        //adding users that isn't contains in database
                        _userChatBinder.AddUsersToChat(addBindings, context);
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
    }
}
