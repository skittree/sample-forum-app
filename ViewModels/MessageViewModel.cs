using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public TopicViewModel Topic { get; set; }
        public string Text { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public DateTime Created { get; set; }
        public IdentityUser Creator { get; set; }
        public DateTime? Modified { get; set; }
    }
}
