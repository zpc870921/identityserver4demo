﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class UserClaims
    {
        [Key]
        public Guid ClaimsId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        
        public Guid UsersId { get; set; }
    }
}
