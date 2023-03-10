using Microsoft.AspNetCore.Identity;
using SuscriptionApp.Enums;

namespace SuscriptionApp.Entities
{
    public class KeyAPI
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public KeyType KeyType{ get; set; }
        public bool Enable { get; set; }
        public string UserId { get; set; }
        public IdentityUser User{ get; set; }
    }
}