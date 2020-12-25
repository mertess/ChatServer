using ServerBusinessLogic.Enums;
using ServerBusinessLogic.Models;

namespace ServerBusinessLogic.ResponseModels.UserModels
{
    public class UserResponseModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string SecondName { get; set; }

        public Gender? Gender { get; set; }

        public string PhoneNumber { get; set; }

        public Country? Country { get; set; }

        public City? City { get; set; }

        public FileModel File { get; set; }

        public bool IsOnline { get; set; }
    }
}
