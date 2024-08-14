using System.ComponentModel.DataAnnotations;

namespace BackEndAPI.Core.Dtos.Auth
{
    public class RegisterDto
    {

        public string FirstName { get; set; }
        public string Lastname { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Address { get; set; }
    }
}
