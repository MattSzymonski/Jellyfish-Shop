using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Id { get; set; }

        public string AccessToken { get; set; }
    }

    public class LoginRequestDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

