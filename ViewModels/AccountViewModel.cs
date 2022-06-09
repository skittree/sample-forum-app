using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Task3.ViewModels
{
    public class AccountViewModel
    {
        public string UserName { get; set; }
        public IdentityUser User { get; set; }
    }
}
