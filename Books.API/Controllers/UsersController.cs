using AutoMapper;
using Books.API.Extensions;
using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Models;
using Books.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;

namespace Books.API.Controllers
{
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [ServiceFilter(typeof(RequestAuthActionFilterAttribute))]
    [TypeFilter(typeof(ApiKeyAuthorizationFilterAttribute))]
    public class UsersController : ControllerBase
    {
        private readonly ProjectOptions _projectOptions;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IOptionsMonitor<ProjectOptions> projectOptions, IUserRepository userRepository, IMapper mapper)
        {
            _projectOptions = projectOptions.CurrentValue;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<GenTokens>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            
            try
            {
                Log.Information("info");

                ServiceResponse<GenTokens> response = await _userRepository.Authenticate(loginRequestDto);
                

                return response.Code switch
                {
                    HttpStatusCode.Created => CreatedAtRoute("", response),
                    HttpStatusCode.Unauthorized => Unauthorized(_mapper.Map<ServiceFailedResponse>(response)),
                    HttpStatusCode.BadRequest => BadRequest(response),
                    HttpStatusCode.Conflict => Conflict(response),
                    HttpStatusCode.InternalServerError => StatusCode(500, response),

                    _ => StatusCode(422, response),
                };





            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ServiceFailedResponse))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<LoginResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
          
            try
            {
                Log.Information("info");

                ServiceResponse<LoginResponseDto> response = await _userRepository.Register(registerRequestDto);
  

                return response.Code switch
                {
                    HttpStatusCode.Created => CreatedAtRoute("", response),
                    HttpStatusCode.Conflict => Conflict(_mapper.Map<ServiceFailedResponse>(response)),
                    HttpStatusCode.BadRequest => BadRequest(response),
                    HttpStatusCode.InternalServerError => StatusCode(500, response),

                    _ => StatusCode(422, response),
                };







            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<LoginResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
        [HttpPost("referesh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
           
            try
            {
                Log.Information("info");

                ServiceResponse<GenTokens> response = await _userRepository.RefreshToken(refreshTokenRequestDto);

                return response.Code switch
                {
                    HttpStatusCode.Created => CreatedAtRoute("", response),
                    HttpStatusCode.Conflict => Conflict(_mapper.Map<ServiceFailedResponse>(response)),
                    HttpStatusCode.BadRequest => BadRequest(response),
                    HttpStatusCode.InternalServerError => StatusCode(500, response),

                    _ => StatusCode(422, response),
                };

            }
            catch (Exception ex)
            {

                Log.Error($"{ex}");
                ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }




    }
}
