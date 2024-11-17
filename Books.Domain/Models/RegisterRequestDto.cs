using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 7)]
        [DefaultValue("ade@yahoo.com")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? LastName { get; set; }


        [Required]
        [DefaultValue("ade")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [DefaultValue("ade1234")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(7, ErrorMessage = "{0} must be at least 7 characters long")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(7, ErrorMessage = "{0} must be at least 7 characters long")]
        [DataType(DataType.Password)]
        [DefaultValue("ade1234")]
        [Compare("Password", ErrorMessage = "password and confirmation Password must match.")]
        public string? ConfirmPassword { get; set; }
    }
}
