﻿namespace ServerBusinessLogic.ResponseModels.NotificationModels
{
    public class NotificationResponseModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public int FromUserId { get; set; }

        public string FromUserName { get; set; }

        public byte[] UserPicture { get; set; }
    }
}
