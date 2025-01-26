using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [ServiceFilter(typeof(RequestAuthActionFilterAttribute))]
    [TypeFilter(typeof(ApiKeyAuthorizationFilterAttribute))]//basicautorization for API
    public class StudentController : ControllerBase
    {

        #region InMemmory-DataSource
        private readonly List<Student> _students =
        [
            new Student
            {
                Id = 1,
                Age=23,
                Name="Adeola"
               
            },
            new Student
            {
                Id = 7,
                Age=12,
                Name="Mariam"
               
            }
        ];
        #endregion






        /// <summary>
        ///get patient records
        /// </summary>ServiceFailedResponse
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<Student>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentName()
        {
            return  Ok( new ServiceResponse<IEnumerable<Student>>
            {
                Code = System.Net.HttpStatusCode.OK,
                IsSuccess = true,
                Message = "successful",
                Data= _students

            });
        }
    }
}
