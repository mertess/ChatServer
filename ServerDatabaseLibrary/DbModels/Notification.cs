﻿using System.ComponentModel.DataAnnotations;

namespace ServerDatabaseSystem.DbModels
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public int ToUserId { get; set; }

        public int FromUserId { get; set; }
        public virtual User FromUser { get; set; }

        [Required]
        public bool IsAccepted { get; set; }
    }
}
