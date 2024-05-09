using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.Contract.Models
{
    public class RegisterUserModel
    {
        public string? UserName { get; set; }

        public string? NickName { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        [Required]
        [AllowNull]
        public string PhoneNumber { get; set; }
    }
}
