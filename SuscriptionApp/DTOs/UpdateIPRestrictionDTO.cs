using System.ComponentModel.DataAnnotations;

namespace SuscriptionApp.DTOs
{
    public class UpdateIPRestrictionDTO
    {
        [Required]
        public string IP { get; set; }
    }
}
