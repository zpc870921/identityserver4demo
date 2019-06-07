using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class Users
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime SubTime { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public virtual ICollection<UserClaims> UserClaims { get; set; }
    }
}
