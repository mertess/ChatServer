using ChatTCPServer.Interfaces;
using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
{
    public class RelatChatUserLogic : IRelatChatUserLogic
    {
        public void AddUserToChat(User user, Chat chat)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                if (context.RelatChatUsers.FirstOrDefault(rcu => rcu.UserId == user.Id && rcu.ChatId == chat.Id) != null)
                    throw new Exception("Пользователь уже находится в данном чате");
                context.RelatChatUsers.Add(new RelatChatUsers() { UserId = user.Id, ChatId = chat.Id });

                //добавление владельца чата (создателя)
                Chat ch = context.Chats.FirstOrDefault(c => c.Id == chat.Id);
                if (!ch.OwnerId.HasValue)
                    ch.OwnerId = user.Id;
                context.SaveChangesAsync();
            }
        }

        public void RemoveUserFromChat(User user, Chat chat)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                RelatChatUsers relatChatUsers = context.RelatChatUsers.FirstOrDefault(rcu => rcu.UserId == user.Id && rcu.ChatId == chat.Id);
                if (relatChatUsers == null)
                    throw new Exception("Ошибка удаления пользователя из чата, пользователь не находися в данном чате");
                context.RelatChatUsers.Remove(relatChatUsers);
                context.SaveChangesAsync();
            }
        }
    }
}
