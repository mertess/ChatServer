using ServerBusinessLogic.Models;
using System;

namespace ServerBusinessLogic.ResponseModels.MessageModels
{
    public class MessageResponseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ChatId { get; set; }

        public string UserMassage { get; set; }

        public DateTime Date { get; set; }

        public FileModel File { get; set; }
    }
}
