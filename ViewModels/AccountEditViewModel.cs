using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Task3.ViewModels
{
    public class AccountEditViewModel
    {
        public string UserName { get; set; }
        public List<SectionViewModel> Sections { get; set; }
        public List<int> SelectedSections { get; set; }
    }
}
