using ServerBusinessLogic.ReceiveModels.NotificationModels;
using ServerBusinessLogic.ReceiveModels.UserModels;
using ServerBusinessLogic.ResponseModels.NotificationModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface INotificationLogic
    {
        void Create(NotificationReceiveModel model);

        void Update(NotificationReceiveModel model);

        void Delete(NotificationReceiveModel model);

        List<NotificationResponseModel> ReadPage(UserPaginationReceiveModel model);

        NotificationResponseModel GetNotification(NotificationReceiveModel model);
    }
}
