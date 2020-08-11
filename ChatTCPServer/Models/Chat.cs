using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Models
{
    public class Chat
    {
        public int Id { get; set; }
        [Required]
        public string ChatName { get; set; }
        [Required]
        public int CountUsers { get; set; }
        [ForeignKey("ChatId")]
        public virtual List<RelatChatUsers> RelatChatUsers { get; set; }
    }
}
