using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Task3.ViewModels
{
    public class MessageEditViewModel
    {
        public int Id { get; set; }
        public int? TopicId { get; set; }
        [Required(ErrorMessage = "Message text cannot be empty.")]
        [Display(Name = "Message Text")]
        public string Text { get; set; }
        [Display(Name = "Add Attachments")]
        public List<IFormFile> Attachments { get; set; }
        public DateTime? Modified { get; set; }
    }
}
