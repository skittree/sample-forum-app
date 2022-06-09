using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class TopicCreateViewModel
    {
        public int SectionId { get; set; }
        public SectionViewModel Section { get; set; }

        [Required(ErrorMessage = "Topic name cannot be empty.")]
        [Display(Name = "Topic Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public DateTime Created { get; set; }
    }
}
