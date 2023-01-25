using Microsoft.AspNetCore.Mvc;

namespace SuscriptionApp.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        protected string GetUserId()
        {
            var userClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var userId = userClaim.Value.ToString();

            return userId;
        }
    }
}
