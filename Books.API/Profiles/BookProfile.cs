using AutoMapper;
using Books.domain.Models;
using Books.Domain.Dto;
using Books.Domain.Entities;
using Books.Domain.Models;

namespace Books.API.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            #region Mapping Left to right

            #endregion  
            CreateMap<ServiceResponse<IEnumerable<BookDto>>, ServiceFailedResponse>();

            CreateMap<ServiceResponse<BookDto>, ServiceFailedResponse>();

            CreateMap<ServiceResponse<Book?>, ServiceFailedResponse>();

            CreateMap<Product, ProductDto>();

            CreateMap<ServiceResponse<ProductDto>, ServiceFailedResponse>();

            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
                $"{src.Author.FirstName} {src.Author.LastName}"))
                .ConstructUsing(src => new BookDto(src.Id,
                string.Empty,
                src.Description,
                src.Title));
            CreateMap<BookForCreationDto, Book>()
                 .ConstructUsing(src => new Book(Guid.NewGuid(),
                src.AuthorId,
                src.Description,
                src.Title));
        }
    }
}
