using AutoMapper;
using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        private readonly ILogger<StudentController> _logger;
        private int _nextId = 1;
        private readonly IMapper _mapper;
        public StudentController(ILogger<StudentController> logger,IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }







        /// <summary>
        ///get student records
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetStudentName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<Student>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentName()
        {
            return Ok(new ServiceResponse<IEnumerable<Student>>
            {
                Code = System.Net.HttpStatusCode.OK,
                IsSuccess = true,
                Message = "successful",
                Data = CollegeRepository.Student

            });
        }

        /// <summary>
        ///get a student records by Id
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("{id:int:min(1):max(1114)}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<Student>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentById([FromRoute] int id)
        {
            Student student = CollegeRepository.Student.FirstOrDefault(student => student.Id.Equals(id));
            if (student != null)
                return Ok(new ServiceResponse<Student>
                {
                    Code = System.Net.HttpStatusCode.OK,
                    IsSuccess = true,
                    Message = "successful",
                    Data = student

                });
            return NotFound(new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.NotFound,
                IsSuccess = false,
                Message = "failed"

            });
        }



        /// <summary>
        ///get a student records by name
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("{name:alpha}",Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<Student>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentByName([FromRoute] string name)
        {
            Student student = CollegeRepository.Student.FirstOrDefault(student => student.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (student != null)
                return Ok(new ServiceResponse<Student>
                {
                    Code = System.Net.HttpStatusCode.OK,
                    IsSuccess = true,
                    Message = "successful",
                    Data = student

                });
            return NotFound(new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.NotFound,
                IsSuccess = false,
                Message = "failed"

            });
        }




        /// <summary>
        ///delete  student records by Id
        /// </summary>
        /// <returns>204</returns>
        [HttpDelete("{id:int:min(1):max(1114)}",Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(ServiceResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult DeleteStudentById([FromRoute] int id)
        {
            try
            {
                Student student = CollegeRepository.Student.FirstOrDefault(student => student.Id.Equals(id));
                if (student != null)
                    return CollegeRepository.Student.Remove(student) ? NoContent()
                        : StatusCode(500, new ServiceFailedResponse()
                        {
                            Code = (int)HttpStatusCode.InternalServerError,
                            IsSuccess = false,
                            Message = "issue deleting records"
                        });
                return NotFound(new ServiceFailedResponse
                {
                    Code = (int)System.Net.HttpStatusCode.NotFound,
                    IsSuccess = false,
                    Message = "failed"

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteStudentById");
            }
            return StatusCode(500, new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.InternalServerError,
                IsSuccess = false,
                Message = "failed"

            });
        }



        /// <summary>
        ///get a student records by name
        /// </summary>
        /// <returns>200</returns>
        [HttpPost(Name = "CreateStudent")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<Student>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        public IActionResult CreateStudent([FromBody] StudentForCreation student)
        {
            var addedStudent = Add(student);
            return CreatedAtAction(nameof(GetStudentById), new { id = addedStudent.Id }, addedStudent);

        }

        [NonAction]
        public Student Add(StudentForCreation studentForCreation)
        {
            Student student = _mapper.Map<Student>(studentForCreation);
            student.Id = GetNextId();
            CollegeRepository.Student.Add(student);
            return student; // Return the added student
        }

        [NonAction]
        private static int GetNextId()
        {
            return !CollegeRepository.Student.Any() ? 1 : CollegeRepository.Student.Max(s => s.Id) + 1;
        }
    }
}
