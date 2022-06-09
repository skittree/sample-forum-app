using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class MessageDeleteViewModel
    {
        public int Id { get; set; }
        public int? TopicId { get; set; }
        public string Text { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public DateTime? Modified { get; set; }
    }
}
