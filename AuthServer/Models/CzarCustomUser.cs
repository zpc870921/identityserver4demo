using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class CzarCustomUser
    {
        public int iid { get; set; }
        public string username { get; set; }
        public string usertruename { get; set; }
        public string userpwd { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
