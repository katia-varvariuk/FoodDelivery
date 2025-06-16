using System.Collections.Generic;

namespace FoodDelivery.BLL.DTOs
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public List<string> Errors { get; set; }
    }
}