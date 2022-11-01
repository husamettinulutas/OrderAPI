using System;

namespace API.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public String FullName => Firstname + ' ' + Lastname;
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public bool GdprApprove { get; set; }
    }
}
