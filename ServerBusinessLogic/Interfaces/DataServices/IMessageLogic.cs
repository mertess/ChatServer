﻿using ServerBusinessLogic.ReceiveModels;
using ServerBusinessLogic.ReceiveModels.ChatModels;
using ServerBusinessLogic.ReceiveModels.MessageModels;
using ServerBusinessLogic.ResponseModels;
using ServerBusinessLogic.ResponseModels.MessageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerBusinessLogic.Interfaces.DataServices
{
    public interface IMessageLogic
    {
        void AddMessage(MessageReceiveModel message);

        void DeleteMessage(MessageReceiveModel message);

        void Update(MessageReceiveModel message);

        List<MessageResponseModel> ReadPage(ChatPaginationReceiveModel chat);

        MessageResponseModel ReadMessage(MessageUserReceiveModel model);
    }
}