using Microsoft.EntityFrameworkCore;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.Models;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

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
            using (DatabaseContext context = new DatabaseContext())
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
        public UserResponseModel GetUser(UserReceiveModel user, bool authorizationSync)
        {
            using (var context = new DatabaseContext())
            {
                var userDb = context.Users
                    .FirstOrDefault(u => user.Id.HasValue && u.Id == user.Id.Value
                || u.Login.Equals(user.Login) && u.Password.Equals(user.Password));

                if (userDb == null)
                    throw new Exception("Пользователь не найден, возможно неправильный логин или пароль");

                if (authorizationSync)
                    userDb.IsOnline = true;

                context.SaveChanges();

                return new UserResponseModel()
                {
                    Id = userDb.Id,
                    UserName = userDb.UserName,
                    Name = userDb.Name,
                    SecondName = userDb.SecondName,
                    Gender = userDb.Gender,
                    Country = userDb.Country,
                    City = userDb.City,
                    IsOnline = userDb.IsOnline,
                    PhoneNumber = userDb.PhoneNumber,
                    File = new FileModel()
                    {
                        FileName = userDb.PictureName,
                        Extension = userDb.PictureExtension,
                        BinaryForm = userDb.Picture
                    }
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
            using (DatabaseContext context = new DatabaseContext())
            {
                return string.IsNullOrEmpty(userModel.SearchingUserName)
                    ? context.Users
                    .Where(u => u.Id != userModel.UserId)
                    .Skip(userModel.Page * 15)
                    .Take(15)
                    .Select(u => new UserListResponseModel()
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        Picture = new FileModel()
                        {
                            FileName = u.PictureName,
                            Extension = u.PictureExtension,
                            BinaryForm = u.Picture
                        },
                        IsOnline = u.IsOnline
                    })
                    .ToList()
                    : context.Users
                    .Where(u => u.Id != userModel.UserId)
                    .ToList()
                    .Where(u => u.UserName.StartsWith(userModel.SearchingUserName, true, CultureInfo.InvariantCulture))
                    .Skip(userModel.Page * 15)
                    .Take(15)
                    .Select(u => new UserListResponseModel()
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        Picture = new FileModel()
                        {
                            FileName = u.PictureName,
                            Extension = u.PictureExtension,
                            BinaryForm = u.Picture
                        },
                        IsOnline = u.IsOnline
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
            using (DatabaseContext context = new DatabaseContext())
            {
                if (!userModel.Id.HasValue)
                    throw new Exception("Ошибка передачи данных, поле Id модели не было установлено");

                User usr = context.Users.FirstOrDefault(u => u.Id == userModel.Id.Value);
                if (usr == null)
                    throw new Exception("Такого пользователя нет в БД");

                usr.Password = userModel.Password ?? usr.Password;
                usr.UserName = userModel.UserName ?? usr.UserName;
                usr.PhoneNumber = userModel.PhoneNumber ?? usr.PhoneNumber;
                usr.Name = userModel.Name ?? usr.Name;
                usr.SecondName = userModel.SecondName ?? usr.SecondName;
                usr.Country = userModel.Country ?? usr.Country;
                usr.Gender = userModel.Gender ?? usr.Gender;
                usr.City = userModel.City ?? usr.City;

                usr.PictureName = userModel.File?.FileName;
                usr.PictureExtension = userModel.File?.Extension;
                usr.Picture = userModel.File?.BinaryForm;

                usr.IsOnline = userModel.IsOnline ?? usr.IsOnline;
                context.SaveChanges();
            }
        }
    }
}
