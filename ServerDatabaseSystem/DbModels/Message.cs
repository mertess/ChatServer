using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseSystem.DbModels
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public string UserMessage { get; set; }
        public virtual Chat Chat { get; set; }
        public virtual User User { get; set; }
    }
}
