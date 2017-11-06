using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HomeWallet_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Currency { get; set; }
    }
}
