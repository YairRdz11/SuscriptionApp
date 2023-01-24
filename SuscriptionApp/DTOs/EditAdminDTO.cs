using System.ComponentModel.DataAnnotations;

namespace SuscriptionApp.DTOs
{
    public class EditAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
