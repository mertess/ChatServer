using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ResponseModels;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.Implementation
{
    /// <summary>
    /// Service for working with Users database table 
    /// </summary>
    public class UserLogic : IUserLogic
    {
        /// <summary>
        /// Creating a new user in Users database table
        /// </summary>
        /// <param name="userModel"><see cref="UserReceiveModel"/></param>
        public void Create(UserReceiveModel userModel)
        {
            using (DatabaseContext context = new DatabaseContext())
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
                    Password = userModel.Password,
                    Picture = new byte[10]
                });
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting a user from Users database table
        /// </summary>
        /// <param name="userModel"><see cref="UserReceiveModel"/></param>
        public void Delete(UserReceiveModel userModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(userModel.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");
                context.Users.Remove(usr);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get page of users by prefix of userName or without prefix
        /// </summary>
        /// <param name="userModel"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns><see cref="UserListResponseModel"/></returns>
        public List<UserListResponseModel> Read(UserPaginationReceiveModel userModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                return string.IsNullOrEmpty(userModel.SearchingUserName)
                    ? context.Users
                    .Skip(userModel.Page * 10)
                    .Take(10)
                    .Select(u => new UserListResponseModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Picture = u.Picture
                    })
                    .ToList()
                    : context.Users
                    .Where(u => u.UserName.StartsWith(userModel.SearchingUserName, true, CultureInfo.InvariantCulture))
                    .Skip(userModel.Page * 10)
                    .Take(10)
                    .Select(u => new UserListResponseModel()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Picture = u.Picture
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating a user in Users database table
        /// </summary>
        /// <param name="userModel"><see cref="UserReceiveModel"/></param>
        public void Update(UserReceiveModel userModel)
        {
            using(DatabaseContext context = new DatabaseContext())
            {
                User usr = context.Users.FirstOrDefault(u => u.Login.Equals(userModel.Login));
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");

                usr.Login = userModel.Login;
                usr.Password = userModel.Password;
                usr.UserName = userModel.UserName;
                usr.Picture = userModel.Picture;
                usr.PhoneNumber = userModel.PhoneNumber;
                usr.SecondName = userModel.SecondName;
                usr.Country = userModel.Country;
                usr.Gender = userModel.Gender;

                context.SaveChanges();
            }
        }
    }
}
