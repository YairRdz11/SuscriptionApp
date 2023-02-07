using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuscriptionApp.DTOs;
using SuscriptionApp.Entities;

namespace SuscriptionApp.Controllers
{
    [Route("api/iprestrictions")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IPRestrictionsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public IPRestrictionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateIPRestrictionDTO createIPRestrictionDTO)
        {
            var keyDB = await context.KeysAPI.FirstOrDefaultAsync(x => x.Id == createIPRestrictionDTO.KeyId);

            if (keyDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if(userId != keyDB.UserId)
            {
                return Forbid();
            }

            var ipReestriction = new IPRestriction
            {
                KeyId = keyDB.Id,
                IP = createIPRestrictionDTO.IP
            };
            context.Add(ipReestriction);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, UpdateIPRestrictionDTO updateIPRestrictionDTO)
        {
            var restrictionDB = await context.IPRestrictions
                .Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(restrictionDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if(restrictionDB.Key.UserId != userId)
            {
                return Forbid();
            }

            restrictionDB.IP = updateIPRestrictionDTO.IP;
            await context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDB = await context.IPRestrictions
                .Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(restrictionDB == null)
            {
                return NotFound();
            }
            var userId = GetUserId();
            if(userId != restrictionDB.Key.UserId)
            {
                return Forbid();
            }

            context.Remove(restrictionDB);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
