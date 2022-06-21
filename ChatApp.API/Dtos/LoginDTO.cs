using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Dtos
{
    public class LoginDTO
    {
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
    }
}
