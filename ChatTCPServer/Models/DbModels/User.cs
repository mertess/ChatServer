using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCPServer.Models.DbModels
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string SecondName { get; set; }
        [Required]
        public string Password { get; set; }
        [ForeignKey("UserId")]
        public virtual List<RelatChatUsers> RelatChatUsers { get; set; }
        [ForeignKey("UserId")]
        public virtual List<Message> Messages { get; set; }
    }
}
