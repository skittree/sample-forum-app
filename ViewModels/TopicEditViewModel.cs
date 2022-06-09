using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class TopicEditViewModel
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        [Required(ErrorMessage = "Topic name cannot be empty.")]
        [Display(Name = "Topic Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
