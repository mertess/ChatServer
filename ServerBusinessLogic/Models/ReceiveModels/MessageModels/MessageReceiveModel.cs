using ServerBusinessLogic.Models;
using System;
using System.IO;

namespace ServerBusinessLogic.ReceiveModels.MessageModels
{
    public class MessageReceiveModel
    {
        public int? Id { get; set; }

        public int FromUserId { get; set; }

        public int ChatId { get; set; }

        public DateTime Date { get; set; }

        public string UserMassage { get; set; }

        public bool IsReaded { get; set; }

        public FileModel File { get; set; }
    }
}
