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
        public User User{ get; set; }
        public List<DomainRestriction> DomainRestrictions { get; set; }
        public List<IPRestriction> IPRestrictions { get; set; }
    }
}