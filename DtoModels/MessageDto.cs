using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.DtoModels
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int? TopicId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public IdentityUser Creator { get; set; }
        public DateTime? Modified { get; set; }
    }
}
