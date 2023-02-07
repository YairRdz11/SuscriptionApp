using System.ComponentModel.DataAnnotations;

namespace SuscriptionApp.DTOs
{
    public class CreateIPRestrictionDTO
    {
        public int KeyId { get; set; }
        [Required]
        public string IP { get; set; }
    }
}
