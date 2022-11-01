namespace API.Dtos
{
    public class RegisterDto : LoginDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool GdprApprove { get; set; }
    }
}
