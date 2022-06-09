using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Task3.ViewModels
{
    public class MessageCreateViewModel
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public TopicViewModel Topic { get; set; }
        [Required(ErrorMessage = "Message text cannot be empty.")]
        [Display(Name = "Message Text")]
        public string Text { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
