using System.ComponentModel.DataAnnotations;

namespace SuscriptionApp.DTOs
{
    public class UpdateDomainRestrictionDTO
    {
        [Required]
        public string Domain { get; set; }
    }
}
