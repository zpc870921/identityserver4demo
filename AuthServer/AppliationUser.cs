using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthServer
{
    public class ApplicationUser:IdentityUser
    {
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public string Address { get; set; }
        public string IdCode { get; set; }
    }
}
