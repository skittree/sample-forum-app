using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Task3.Store.Roles
{
    public static class Roles
    {
        public static readonly string Admin = "Admin";
        public static readonly string Moderator = "Moderator";
        public static readonly string[] AllRoles = new string[] { Admin, Moderator };
    }
}
