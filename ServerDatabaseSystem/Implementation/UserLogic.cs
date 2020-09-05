using ServerBusinessLogic.Interfaces;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.Implementation
{
    public class UserLogic : IUserLogic
    {
        public void Create(UserReceiveModel userModel)
        {
            using (ChatDatabaseContext context = new ChatDatabaseContext())
            {
                if (context.Users.FirstOrDefault(u => u.Login.Equals(userModel.Login)) != null)
                    throw new Exception("Этот логин уже зарегистрирован!");
                context.Users.Add(new User()
                {
                    Id = userModel.Id,
                    UserName = userModel.UserName,
                    Name = userModel.Name,
                    SecondName = userModel.SecondName,
                    Login = userModel.Login,
                    Password = userModel.Password
                });
                context.SaveChanges();
            }
        }

        public void Delete(UserReceiveModel userModel)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(userModel.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");
                context.Users.Remove(usr);
                context.SaveChanges();
            }
        }

        public List<UserResponseModel> Read(UserReceiveModel userModel)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                return context.Users.Where(u => userModel == null 
                || userModel.Login.Equals(u.Login) && userModel.Password.Equals(u.Password))
                    .Select(u => new UserResponseModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Name = u.Name,
                        SecondName = u.SecondName,
                        Login = u.Login,
                        Password = u.Password
                    })
                    .ToList();
            }
        }

        public void Update(UserReceiveModel userModel)
        {
            using(ChatDatabaseContext context = new ChatDatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(userModel.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");
                //пускай пока будет обновление логина, пароля и никнейма
                usr.Login = userModel.Login;
                usr.Password = userModel.Password;
                usr.UserName = userModel.UserName;
                context.SaveChanges();
            }
        }
    }
}
