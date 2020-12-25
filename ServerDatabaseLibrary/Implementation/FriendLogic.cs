using Microsoft.EntityFrameworkCore;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.Models;
using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ServerDatabaseSystem.Implementation
{
    /// <summary>
    /// Service for working with Friends database table
    /// </summary>
    public class FriendLogic : IFriendLogic
    {
        /// <summary>
        /// Adding a new friend to Friends database table 
        /// </summary>
        /// <param name="model"><see cref="FriendReceiveModel"/></param>
        public void Create(FriendReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                if (context.Friends.FirstOrDefault(f => f.UserId == model.UserId && f.FriendId == model.FriendId) != null)
                    throw new Exception("Данный пользователь уже находится у вас в друзьях");

                context.Friends.AddRange(
                    new DbModels.Friend()
                    {
                        UserId = model.UserId,
                        FriendId = model.FriendId
                    }, 
                    new DbModels.Friend()
                    {
                        UserId = model.FriendId,
                        FriendId = model.UserId
                    });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting a friend from Friends database table
        /// </summary>
        /// <param name="model"><see cref="FriendReceiveModel"/></param>
        public void Delete(FriendReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                var friend = context.Friends.FirstOrDefault(f => f.UserId == model.UserId && f.FriendId == model.FriendId);
                if (friend == null)
                    throw new Exception("Пользователь не найден у вас в друзьях");

                //unbinding current user with his friend 
                context.Friends.RemoveRange(
                        friend, 
                        context.Friends.FirstOrDefault(f => f.UserId == model.FriendId && f.FriendId == model.UserId)
                        );
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Get page of friends of user by Id
        /// </summary>
        /// <param name="model"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns><see cref="UserListResponseModel"/></returns>
        public List<UserListResponseModel> ReadPage(UserPaginationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                return string.IsNullOrEmpty(model.SearchingUserName) ?
                    context.Friends
                    .Where(f => f.UserId == model.UserId)
                    .Skip(model.Page * 10)
                    .Take(10)
                    //danger
                    .Select(f => context.Users.FirstOrDefault(u => u.Id == f.FriendId))
                    .Select(u => new UserListResponseModel()
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        Picture = new FileModel()
                        {
                            FileName = u.PictureName,
                            Extension = u.PictureExtension,
                            BinaryForm = u.Picture
                        }
                    })
                    .ToList()
                    :
                    context.Friends
                    .Where(f => f.UserId == model.UserId)
                    .Select(f => context.Users.FirstOrDefault(u => u.Id == f.FriendId))
                    .ToList()
                    .Where(u => u.UserName.StartsWith(model.SearchingUserName, true, CultureInfo.InvariantCulture))
                    .Skip(model.Page * 10)
                    .Take(10)
                    .Select(u => new UserListResponseModel()
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        Picture = new FileModel()
                        {
                            FileName = u.PictureName,
                            Extension = u.PictureExtension,
                            BinaryForm = u.Picture
                        }
                    })
                    .ToList();
            }
        }
    }
}
