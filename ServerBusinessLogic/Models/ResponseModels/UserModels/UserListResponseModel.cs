using ServerBusinessLogic.Models;

namespace ServerBusinessLogic.ResponseModels.UserModels
{
    public class UserListResponseModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public FileModel Picture { get; set; }

        public bool IsOnline { get; set; }
    }
}
