using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class AttachmentViewModel
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public MessageViewModel Message { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
    }
}
