namespace ServerBusinessLogic.ResponseModels.UserModels
{
    public class UserListResponseModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public byte[] Picture { get; set; }
    }
}
