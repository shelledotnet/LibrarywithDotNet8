using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Dto
{
    public class CountryDto
    {
        [Required(ErrorMessage = "{0} required")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50, ErrorMessage = "{0} max length is 50"), MinLength(3, ErrorMessage = "{0} min length is 3")]
        [DefaultValue("japan")]
        public string? Country { get; set; }
    }
}
