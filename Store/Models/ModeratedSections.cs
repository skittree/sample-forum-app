using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Task3.Store.Models
{
    public class ModeratedSections
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
    }
}
