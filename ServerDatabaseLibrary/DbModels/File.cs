using ServerDatabaseSystem.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabaseLibrary.DbModels
{
    public class File
    {
        public int Id { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public byte[] BinaryForm { get; set; }

        [ForeignKey("FileId")]
        public virtual User User { get; set; }
    }
}
