using ServerBusinessLogic.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerDatabaseSystem.DbModels
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SecondName { get; set; }

        public Gender? Gender { get; set; }

        public string PhoneNumber { get; set; }

        public Country? Country { get; set; }

        public City? City { get; set; }

        public string PictureName { get; set; }

        public string PictureExtension { get; set; }

        public byte[] Picture { get; set; }

        public bool IsOnline { get; set; }

        [ForeignKey("UserId")]
        public virtual List<RelationChatUser> RelatChatUsers { get; set; }

        [ForeignKey("UserId")]
        public virtual List<Friend> Friends { get; set; }

        [ForeignKey("FromUserId")]
        public virtual List<Notification> Notifications { get; set; }

        [ForeignKey("CreatorId")]
        public virtual List<Chat> Chats { get; set; }

        [ForeignKey("FromUserId")]
        public virtual List<Message> Messages { get; set; }
    }
}
