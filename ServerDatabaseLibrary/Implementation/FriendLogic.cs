using Microsoft.EntityFrameworkCore.Storage;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.ReceiveModels.FriendModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.UserModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

                context.Friends.Add(new DbModels.Friend()
                {
                    UserId = model.UserId,
                    FriendId = model.FriendId
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

                context.Friends.Remove(friend);
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
                        Id = u.Id,
                        UserName = u.UserName,
                        Picture = u.Picture
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
                        Id = u.Id,
                        UserName = u.UserName,
                        Picture = u.Picture
                    })
                    .ToList();
            }
        }
    }
}
