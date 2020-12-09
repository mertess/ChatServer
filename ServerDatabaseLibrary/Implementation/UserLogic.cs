using Microsoft.EntityFrameworkCore.Storage;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels;
using ServerBusinessLogic.ResponseModels.UserModels;
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
                    UserName = userModel.UserName,
                    Name = userModel.Name,
                    SecondName = userModel.SecondName,
                    Login = userModel.Login,
                    Password = userModel.Password
                    //Picture = new byte[10]
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
        /// Get a user by Id or login and password
        /// </summary>
        /// <param name="user"><see cref="UserReceiveModel"/></param>
        /// <returns><see cref="UserResponseModel"/></returns>
        public UserResponseModel GetUser(UserReceiveModel user)
        {
            using(var context = new DatabaseContext())
            {
                var userDb = context.Users.FirstOrDefault(u => user.Id.HasValue && u.Id == user.Id.Value
                || u.Login.Equals(user.Login) && u.Password.Equals(user.Password));

                return userDb == null ? null : new UserResponseModel()
                {
                    Id = userDb.Id,
                    UserName = userDb.UserName,
                    Name = userDb.Name,
                    SecondName = userDb.SecondName,
                    Gender = userDb.Gender.Value,
                    Country = userDb.Country.Value,
                    City = userDb.City.Value,
                    IsOnline = userDb.IsOnline,
                    PhoneNumber = userDb.PhoneNumber,
                    Picture = userDb.Picture
                };
            }
        }

        /// <summary>
        /// Get page of users by prefix of userName or without prefix
        /// </summary>
        /// <param name="userModel"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns><see cref="UserListResponseModel"/></returns>
        public List<UserListResponseModel> ReadPage(UserPaginationReceiveModel userModel)
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
