using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.Store.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public Section Section { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public IdentityUser Creator { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}
