using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuscriptionApp.Entities;

namespace SuscriptionApp
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<KeyAPI> KeysAPI{ get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<DomainRestriction> DomainRestrictions { get; set; }
        public DbSet<IPRestriction> IPRestrictions { get; set; }
    }
}
