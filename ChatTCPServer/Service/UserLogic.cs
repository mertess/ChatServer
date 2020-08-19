using ChatTCPServer.Interfaces;
using ChatTCPServer.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Service
{
    public class UserLogic : IUserLogic
    {
        public void Create(User user)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                if (context.Users.FirstOrDefault(u => u.Login.Equals(user.Login)) != null)
                    throw new Exception("Этот логин уже зарегистрирован!");
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public void Delete(User user)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(user.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");
                context.Users.Remove(usr);
                context.SaveChangesAsync();
            }
        }

        public List<User> Read(User user)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                return context.Users.Where(u => user == null 
                || user.Login.Equals(u.Login) && user.Password.Equals(u.Password)).ToList();
            }
        }

        public void Update(User user)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(user.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");
                //пускай пока будет обновление логина, пароля и никнейма
                usr.Login = user.Login;
                usr.Password = user.Password;
                usr.UserName = user.UserName;
                context.SaveChangesAsync();
            }
        }
    }
}
