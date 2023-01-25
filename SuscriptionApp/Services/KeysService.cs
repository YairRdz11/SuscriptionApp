using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SuscriptionApp.Entities;
using SuscriptionApp.Enums;

namespace SuscriptionApp.Services
{
    public class KeysService
    {
        private readonly ApplicationDbContext context;

        public KeysService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task BuildKey(string userId, KeyType keyType)
        {
            var key = GenerateKey();

            var keyAPI = new KeyAPI
            {
                Enable = true,
                Key = key,
                KeyType = keyType,
                UserId = userId
            };

            context.Add(keyAPI);
            await context.SaveChangesAsync();
        }

        public string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
