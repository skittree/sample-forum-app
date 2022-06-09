using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.ViewModels
{
    public class TopicDeleteViewModel
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
