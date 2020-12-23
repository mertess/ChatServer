using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerDatabaseSystem.DbModels
{
    public class Chat
    {
        public int Id { get; set; }

        [Required]
        public string ChatName { get; set; }

        [Required]
        public int CreatorId { get; set; }
        public User Creator { get; set; }

        [Required]
        public int CountUsers { get; set; }

        [Required]
        public DateTime DateOfCreation { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [ForeignKey("ChatId")]
        public virtual List<RelationChatUser> RelatChatUsers { get; set; }

        [ForeignKey("ChatId")]
        public virtual List<Message> Messages { get; set; }
    }
}
