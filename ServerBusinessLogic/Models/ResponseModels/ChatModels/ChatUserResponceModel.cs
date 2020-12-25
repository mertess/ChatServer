using ServerBusinessLogic.Models;

namespace ServerBusinessLogic.ResponseModels.ChatModels
{
    public class ChatUserResponseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public FileModel Picture { get; set; }

        public bool IsOnline { get; set; }
    }
}
