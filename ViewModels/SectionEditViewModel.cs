using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class SectionEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Section name cannot be empty.")]
        [Display(Name = "Section Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
