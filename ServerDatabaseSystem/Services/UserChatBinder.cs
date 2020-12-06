using ServerBusinessLogic.ReceiveModels;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerDatabaseSystem.Services
{
    public class UserChatBinder
    {
        public void AddUsersToChat(List<ChatUserReceiveModel> models, DatabaseContext context)
        {
            foreach (var model in models)
            {
                if (context.RelationChatUsers.FirstOrDefault(rcu => rcu.UserId == model.UserId && rcu.ChatId == model.ChatId) != null)
                    throw new Exception("Пользователь уже находится в данном чате");
                context.RelationChatUsers.Add(new RelationChatUser() { UserId = model.UserId, ChatId = model.ChatId });
                context.SaveChanges();
            }
        }

        public void RemoveUsersFromChat(List<ChatUserReceiveModel> models, DatabaseContext context)
        {
            foreach (var model in models)
            {
                RelationChatUser relatChatUser = context.RelationChatUsers.FirstOrDefault(rcu => rcu.UserId == model.UserId && rcu.ChatId == model.ChatId);
                if (relatChatUser == null)
                    throw new Exception("Ошибка удаления пользователя из чата, пользователь не находися в данном чате");
                context.RelationChatUsers.Remove(relatChatUser);
                context.SaveChanges();
            }
        }
    }
}
