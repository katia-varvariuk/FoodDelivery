using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.BLL.DTOs
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}