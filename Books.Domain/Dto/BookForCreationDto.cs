// Ignore Spelling: Dto

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Dto
{
    public class BookForCreationDto
    {
        [Required(ErrorMessage ="{0} is required")]
        public Guid AuthorId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string? Description { get; set; }

        public BookForCreationDto(Guid authorId, string? title, string? description)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
        }

    }
}
