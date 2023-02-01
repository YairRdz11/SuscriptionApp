using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuscriptionApp.DTOs;
using SuscriptionApp.Entities;

namespace SuscriptionApp.Controllers
{
    [Route("api/restrictionsdomain")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DomainRestrictionsController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public DomainRestrictionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateDomainRestrictionsDTO createDomainRestrictionsDTO)
        {
            var keyDB = await context.KeysAPI
                .FirstOrDefaultAsync(x => x.Id == createDomainRestrictionsDTO.KeyId);

            if (keyDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if(userId != keyDB.UserId)
            {
                return Forbid();
            }

            var domainRestriction = new DomainRestriction()
            {
                KeyId = createDomainRestrictionsDTO.KeyId,
                Domain = createDomainRestrictionsDTO.Domain
            };

            context.Add(domainRestriction);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, UpdateDomainRestrictionDTO updateDomainRestrictionDTO)
        {
            var restrictionDB = await context.DomainRestrictions
                .Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (restrictionDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId != restrictionDB.Key.UserId)
            {
                return Forbid();
            }

            restrictionDB.Domain = updateDomainRestrictionDTO.Domain;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDB = await context.DomainRestrictions
                .Include(x => x.Key)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (restrictionDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId != restrictionDB.Key.UserId)
            {
                return Forbid();
            }

            context.Remove(restrictionDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
