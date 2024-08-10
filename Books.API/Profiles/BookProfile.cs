using AutoMapper;
using Books.domain.Models;
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
            CreateMap<ServiceResponse<IEnumerable<Book>>, ServiceFailedResponse>();

            CreateMap<ServiceResponse<Book?>, ServiceFailedResponse>();

            CreateMap<Product, ProductDto>();

            CreateMap<ServiceResponse<ProductDto>, ServiceFailedResponse>();
        }
    }
}
