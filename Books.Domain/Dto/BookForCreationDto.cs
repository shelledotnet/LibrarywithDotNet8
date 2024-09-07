// Ignore Spelling: Dto

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Dto
{
    public class BookForCreationDto
    {
        //this is a value type you dont need required attribute it will always have a min value
        //[Required(ErrorMessage = "{0} is required")]
        [DefaultValue(typeof(Guid), "8DAEDD83-289E-417C-AAAC-D48D77E0D84C")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        public BookForCreationDto(Guid authorId, string? title, string? description)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
        }

    }
}
