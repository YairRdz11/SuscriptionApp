using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuscriptionApp.DTOs;
using SuscriptionApp.Services;

namespace SuscriptionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIKeysControllers : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly KeysService keysService;

        public APIKeysControllers(ApplicationDbContext context,
            IMapper mapper,
            KeysService keysService)
        {
            this.context = context;
            this.mapper = mapper;
            this.keysService = keysService;
        }

        [HttpGet]
        public async Task<List<KeyDTO>> MyKeys()
        {
            var userId = GetUserId();
            var keys = await context.KeysAPI.Where(x => x.UserId == userId).ToListAsync();

            return mapper.Map<List<KeyDTO>>(keys);
        }

        [HttpPost]
        public async Task<ActionResult> CreateKey(CreateKeyDTO createKeyDTO)
        {
            var userId = GetUserId();

            if(createKeyDTO.KeyType == Enums.KeyType.Free)
            {
                var userHasFreeKeyAlready = await context.KeysAPI
                    .AnyAsync(x => x.UserId == userId && x.KeyType == Enums.KeyType.Free);
                if (userHasFreeKeyAlready)
                {
                    return BadRequest("User already has a free key");
                }
            }

            await keysService.BuildKey(userId, createKeyDTO.KeyType);

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateKey(UpdateKeyDTO updateKeyDTO)
        {
            var userId = GetUserId();
            var keyDB = await context.KeysAPI.FirstOrDefaultAsync(x => x.Id == updateKeyDTO.KeyId);

            if(keyDB == null)
            {
                return NotFound();
            }

            if(userId != keyDB.UserId)
            {
                return Forbid();
            }

            if (updateKeyDTO.UpdateKey)
            {
                keyDB.Key = keysService.GenerateKey();
            }

            keyDB.Enable = updateKeyDTO.Enable;
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
