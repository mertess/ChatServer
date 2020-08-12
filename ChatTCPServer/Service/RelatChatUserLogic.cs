using ChatTCPServer.Interfaces;
using ChatTCPServer.Models;
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
                context.RelatChatUsers.Add(new RelatChatUsers() { UserId = user.Id, ChatId = chat.Id });
                context.SaveChangesAsync();
            }
        }

        public void RemoveUserFromChat(User user, Chat chat)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                RelatChatUsers relatChatUsers = context.RelatChatUsers.FirstOrDefault(rcu => rcu.UserId == user.Id && rcu.ChatId == chat.Id);
                if (relatChatUsers == null)
                    throw new Exception("Ошибка удаления пользователя из чата");
                context.RelatChatUsers.Remove(relatChatUsers);
                context.SaveChangesAsync();
            }
        }
    }
}
