using System.Reflection.Metadata.Ecma335;

namespace SuscriptionApp.DTOs
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
