using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.DbModels
{
    public class Message
    {
        public int Id { get; set; }

        public int FromUserId { get; set; }
        public virtual User FromUser { get; set; }

        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }

        [Required]
        public string UserMessage { get; set; }

        [Required]
        public bool IsReaded { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
