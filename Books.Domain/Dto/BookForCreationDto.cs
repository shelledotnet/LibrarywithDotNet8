using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Dto
{
    public class BookForCreationDto
    {
        public Guid AuthorId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public BookForCreationDto(Guid authorId, string? title, string? description)
        {
            AuthorId = authorId;
            Title = title;
            Description = description;
        }

    }
}
