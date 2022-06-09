using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.Store.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public int? TopicId { get; set; }
        public Topic Topic { get; set; }
        [Required]
        public string Text { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }
        public DateTime Created { get; set; }
        public IdentityUser Creator { get; set; }
        public DateTime? Modified { get; set; }
    }
}
