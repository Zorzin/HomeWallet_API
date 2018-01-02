using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HomeWallet_API.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string Nick { get; set; }
        public string Password { get; set; }
        public string Currency { get; set; }
    }
}
