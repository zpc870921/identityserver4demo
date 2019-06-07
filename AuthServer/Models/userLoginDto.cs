using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class userLoginDto
    {
        public string phoneNumber { get; set; }
        public string passWord { get; set; }
    }
}
