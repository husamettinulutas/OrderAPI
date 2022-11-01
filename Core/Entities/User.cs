using Microsoft.AspNetCore.Identity;
using System;

namespace Core.Entities
{
    public class User : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public String FullName => Firstname + ' ' + Lastname;
        public bool GdprApprove { get; set; }

        public virtual UserAddress UserAddress { get; set; } = new UserAddress();
    }
}
