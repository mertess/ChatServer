using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels.NotificationModels
{
    //TODO : изменить данную модель, чтобы она передавала информацию: от кого, картинку и т.д.
    public class NotificationResponseModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public int FromUserId { get; set; }

        public int ToUserId { get; set; }
    }
}
