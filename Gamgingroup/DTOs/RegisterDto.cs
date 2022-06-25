using System.ComponentModel.DataAnnotations;

namespace Gamgingroup.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)] public string Password { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        [Required] public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public int MyProperty { get; set; }
        public string Interests { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

    }
}
