﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels.MessageModels
{
    public class MessageResponseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ChatId { get; set; }

        public string UserMassage { get; set; }

        public DateTime Date { get; set; }
    }
}
