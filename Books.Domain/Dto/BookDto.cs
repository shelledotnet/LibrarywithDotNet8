using Books.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Dto
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string? AuthorName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }


        public BookDto(Guid id, string? title, string? description , string authorName)
        {

            Id = id;
            Title = title;
            Description = description;
            AuthorName = authorName;
        }
    }
}
