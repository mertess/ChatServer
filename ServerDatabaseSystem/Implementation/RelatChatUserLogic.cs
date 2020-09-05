using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.Implementation
{
    public class RelatChatUserLogic : IRelatChatUserLogic
    {
        public void AddUserToChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                if (context.RelatChatUsers.FirstOrDefault(rcu => rcu.UserId == userModel.Id && rcu.ChatId == chatModel.Id) != null)
                    throw new Exception("Пользователь уже находится в данном чате");
                context.RelatChatUsers.Add(new RelatChatUsers() { UserId = userModel.Id, ChatId = chatModel.Id });

                //добавление владельца чата (создателя)
                Chat ch = context.Chats.FirstOrDefault(c => c.Id == chatModel.Id);
                if (!ch.OwnerId.HasValue)
                    ch.OwnerId = userModel.Id;
                context.SaveChanges();
            }
        }

        public void RemoveUserFromChat(UserReceiveModel userModel, ChatReceiveModel chatModel)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                RelatChatUsers relatChatUsers = context.RelatChatUsers.FirstOrDefault(rcu => rcu.UserId == userModel.Id && rcu.ChatId == chatModel.Id);
                if (relatChatUsers == null)
                    throw new Exception("Ошибка удаления пользователя из чата, пользователь не находися в данном чате");
                context.RelatChatUsers.Remove(relatChatUsers);
                context.SaveChanges();
            }
        }
    }
}
