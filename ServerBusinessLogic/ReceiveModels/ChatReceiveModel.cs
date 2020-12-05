﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.ReceiveModels
{
    public class ChatReceiveModel
    {
        public int Id { get; set; }
        public string ChatName { get; set; }
        public int? OwnerId { get; set; }
        public int CountUsers { get; set; }
    }
}