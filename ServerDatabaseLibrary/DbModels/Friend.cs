﻿using System.ComponentModel.DataAnnotations;

namespace ServerDatabaseSystem.DbModels
{
    public class Friend
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int FriendId { get; set; }
    }
}
