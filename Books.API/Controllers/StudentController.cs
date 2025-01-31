using AutoMapper;
using Bogus.DataSets;
using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Books.API.Controllers
{
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<StudentResponseDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentName()
        {
            List<Student> listStudent = CollegeRepository.Student;
            if (listStudent.Count > 0)
                return Ok(new ServiceResponse<IEnumerable<StudentResponseDto>>
                {
                    Code = System.Net.HttpStatusCode.OK,
                    IsSuccess = true,
                    Message = "successful",
                    Data = _mapper.Map<List<StudentResponseDto>>(listStudent)

                });
            return NotFound(new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.NotFound,
                IsSuccess = false,
                Message = "student not found"

            });

        }

        /// <summary>
        ///get a student records by Id
        /// </summary>
        /// <returns>200</returns>
        [HttpGet("{id:int:min(1):max(1114)}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<StudentResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        public IActionResult GetStudentById([FromRoute]int id)
        {
            Student student = CollegeRepository.Student.FirstOrDefault(student => student.Id.Equals(id));
            if (student != null)
                return Ok(new ServiceResponse<StudentResponseDto>
                {
                    Code = System.Net.HttpStatusCode.OK,
                    IsSuccess = true,
                    Message = "successful",
                    Data = _mapper.Map<StudentResponseDto>(student)

                });
            return NotFound(new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.NotFound,
                IsSuccess = false,
                Message = $"The student with Id: {id} not found"

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
            try
            {
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
                    Message = $"The student with name {name} not found"

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetStudentByName));

            }
            return StatusCode(500, new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.InternalServerError,
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
                _logger.LogError(ex, nameof(DeleteStudentById));
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
            try
            {
                var addedStudent = Add(student);
                return CreatedAtAction(nameof(GetStudentById), new { id = addedStudent.Id }, addedStudent);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, nameof(CreateStudent));

            }
            return StatusCode(500, new ServiceFailedResponse
            {
                Code = (int)System.Net.HttpStatusCode.InternalServerError,
                IsSuccess = false,
                Message = "failed"

            });
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
