using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.Store.Models
{
    public class Attachment
    {
        [Key]
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public Message Message { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
    }
}
