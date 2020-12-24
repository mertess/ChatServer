using System;
using System.Collections.Generic;
using System.Text;

namespace ServerBusinessLogic.Models
{
    public class FileModel
    {
        public int? Id { get; set; }

        public string Extension { get; set; }

        public string FileName { get; set; }

        public byte[] BinaryForm { get; set; }
    }
}
