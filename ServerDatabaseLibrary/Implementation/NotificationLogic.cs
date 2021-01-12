using Microsoft.EntityFrameworkCore;
using ServerBusinessLogic.Interfaces.DataServices;
using ServerBusinessLogic.Models;
using ServerBusinessLogic.ReceiveModels.NotificationModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.NotificationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerDatabaseSystem.Implementation
{
    /// <summary>
    /// Service for working with Notifications database table
    /// </summary>
    public class NotificationLogic : INotificationLogic
    {
        /// <summary>
        /// Adding a new notification to Notifications database table
        /// </summary>
        /// <param name="model"><see cref="NotificationReceiveModel"/></param>
        public void Create(NotificationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                //maybe tmp 
                if (context.Notifications.FirstOrDefault(n => n.FromUserId == model.FromUserId && n.ToUserId == model.ToUserId
                && n.Message == model.Message && n.IsAccepted == model.IsAccepted) != null)
                    throw new Exception("Такое уведомление уже есть! тестовая проверка");

                context.Notifications.Add(new DbModels.Notification()
                {
                    FromUserId = model.FromUserId,
                    ToUserId = model.ToUserId,
                    IsAccepted = false,
                    Message = model.Message
                });

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting a notification from Notifications database table
        /// </summary>
        /// <param name="model"><see cref="NotificationReceiveModel"/></param>
        public void Delete(NotificationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                if (model.Id.HasValue)
                {
                    var notification = context.Notifications.FirstOrDefault(n => n.Id == model.Id.Value);
                    if (notification == null)
                        throw new Exception("Удаляемое уведомление не найдено!");

                    context.Notifications.Remove(notification);
                    context.SaveChanges();
                }
                else
                    throw new Exception("Ошибка передачи данных, не было задано свойство Id модели");
            }
        }

        //TODO : изменить модель NotificationResponseModel и изменить метод
        /// <summary>
        /// Get page of notifications of user by Id
        /// </summary>
        /// <param name="model"><see cref="UserPaginationReceiveModel"/></param>
        /// <returns><see cref="NotificationResponseModel"/></returns>
        public List<NotificationResponseModel> ReadPage(UserPaginationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                var notifications = context.Notifications
                    .Where(n => n.ToUserId == model.UserId && n.IsAccepted == false)
                    .Include(n => n.FromUser);

                notifications.Reverse();

                return notifications
                    .Skip(model.Page * 10)
                    .Take(10)
                    .Select(n => new NotificationResponseModel()
                    {
                        Id = n.Id,
                        Message = n.Message,
                        FromUserId = n.FromUserId,
                        FromUserName = n.FromUser.UserName,
                        UserPicture = new FileModel()
                        {
                            FileName = n.FromUser.PictureName,
                            Extension = n.FromUser.PictureExtension,
                            BinaryForm = n.FromUser.Picture
                        }
                    })
                    .ToList();
            }
        }

        /// <summary>
        /// Updating a notification in Notifications database table
        /// </summary>
        /// <param name="model"><see cref="NotificationReceiveModel"/></param>
        public void Update(NotificationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                if (model.Id.HasValue)
                {
                    var notification = context.Notifications.FirstOrDefault(n => n.Id == model.Id.Value);
                    if (notification == null)
                        throw new Exception("Обновляемое уведомление не найдено!");

                    notification.IsAccepted = model.IsAccepted;

                    context.SaveChanges();
                }
                else
                    throw new Exception("Ошибка передачи данных, не было задано свойство Id модели");
            }
        }

        public NotificationResponseModel GetNotification(NotificationReceiveModel model)
        {
            using (var context = new DatabaseContext())
            {
                var notification = context.Notifications
                    .Include(n => n.FromUser)
                    .FirstOrDefault(n => model.Id.HasValue && n.Id == model.Id.Value || n.FromUserId == model.FromUserId && n.ToUserId == model.ToUserId);

                if (notification == null)
                    throw new Exception("Уведомление не найдено");

                return new NotificationResponseModel()
                {
                    Id = notification.Id,
                    FromUserId = notification.FromUserId,
                    FromUserName = notification.FromUser.UserName,
                    Message = notification.Message,
                    UserPicture = new FileModel()
                    {
                        FileName = notification.FromUser.PictureName,
                        Extension = notification.FromUser.PictureExtension,
                        BinaryForm = notification.FromUser.Picture
                    }
                };
            }
        }
    }
}
