using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class BooksId
    {
        [FromQuery]
        [Required(ErrorMessage = "{0} is required")]
        [DefaultValue(typeof(Guid), "8DAEDD83-289E-417C-AAAC-D48D77E0D84C")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid {0} ")]
        //[Range(0, 10000, ErrorMessage = "{0} must be between 0 and 1000")]
        public Guid Id { get; set; }
    }
}
