using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_6
{
    public class OutdoorUser : IdentityUser
    {
        public OutdoorUser(string userName) : base(userName)
        {
        }

        public virtual string Address { get; set; }
    }
}
