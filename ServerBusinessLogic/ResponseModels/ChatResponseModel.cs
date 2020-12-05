﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ResponseModels
{
    public class ChatResponseModel
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public int? OwnerId { get; set; }
        public int CountUsers { get; set; }
    }
}