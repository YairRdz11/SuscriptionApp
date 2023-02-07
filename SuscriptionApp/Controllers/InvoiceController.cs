using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuscriptionApp.DTOs;

namespace SuscriptionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvoiceController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public InvoiceController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Pay(PayInvoiceDTO payInvoiceDTO)
        {
            var invoiceDB = await context.Invoices
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == payInvoiceDTO.InvoiceId);

            if(invoiceDB == null)
            {
                return NotFound();
            }
            if (invoiceDB.Paid)
            {
                return BadRequest("The invoice is paid already");
            }
            //TODO: Logic pay with pasarela

            invoiceDB.Paid = true;
            await context.SaveChangesAsync();

            var areThereInvoicePending = await context.Invoices
                .AnyAsync(
                    x => x.UserId == invoiceDB.UserId 
                    && !x.Paid 
                    && x.PaymentDeadlineDate < DateTime.Today);
            if (!areThereInvoicePending)
            {
                invoiceDB.User.BadUser = false;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
