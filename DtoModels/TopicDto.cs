using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.DtoModels
{
    public class TopicDto
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public IdentityUser Creator { get; set; }
    }
}
