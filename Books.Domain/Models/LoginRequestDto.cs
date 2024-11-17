using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class LoginRequestDto
    {

        [Required(ErrorMessage = "{0} is required")]
        [DefaultValue("adeolas")]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DefaultValue("ade1234")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "{0} max Length is 30"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? Password { get; set; }

    }
}
