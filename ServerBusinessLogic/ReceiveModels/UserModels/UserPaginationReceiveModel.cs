﻿namespace ServerBusinessLogic.ReceiveModels.UserModels
{
    public class UserPaginationReceiveModel
    {
        public int UserId { get; set; }

        public int Page { get; set; }

        public string SearchingUserName { get; set; }
    }
}
