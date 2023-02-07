using Microsoft.AspNetCore.Identity;

namespace SuscriptionApp.Entities
{
    public class User : IdentityUser
    {
        public bool BadUser { get; set; }
    }
}
