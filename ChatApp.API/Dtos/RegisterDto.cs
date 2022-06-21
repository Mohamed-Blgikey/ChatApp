using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Dtos
{
    public class RegisterDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(6)]
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
