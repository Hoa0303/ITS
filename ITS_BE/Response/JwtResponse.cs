namespace ITS_BE.Response
{
    public class JwtResponse
    {
        public string Jwt { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
