using AutoMapper;
using Books.API.Extensions;
using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Dto;
using Books.Domain.Entities;
using Books.Domain.Models;
using Books.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net;

namespace Books.API.Controllers;

[Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
[Consumes("application/json")] //input-formatter Media type: content-type header
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
[ServiceFilter(typeof(RequestAuthActionFilterAttribute))]
[TypeFilter(typeof(ApiKeyAuthorizationFilterAttribute))]

public class BooksController : ControllerBase
{
    private readonly IBooksRepository _booksRepository;
    private readonly ProjectOptions _projectOptions;
    private readonly ILogger<BooksRepository> _logger;
    private readonly IMapper _mapper;


    #region InMemmory-DataSource
    private readonly List<Product> _products = new()
        {
            new Product
            {
                Id = 1,
                Category = "Electronic",
                Brand = "Sony",
                Name = "Play Station",
                WarrantyYears = 2,
                IsAvailable = true
            },
            new Product
            {
                Id = 7,
                Category = "Electronic",
                Brand = "Apple",
                Name = "Mobile",
                WarrantyYears = 2,
                IsAvailable = true
            }
        }; 
    #endregion



    public BooksController(IBooksRepository booksRepository, IOptionsMonitor<ProjectOptions> projectOptions,
            ILogger<BooksRepository> logger, IMapper mapper)
    {
        _booksRepository = booksRepository ??
            throw new ArgumentNullException(nameof(booksRepository));
        _projectOptions = projectOptions.CurrentValue;
        _logger = logger;
        _mapper= mapper ?? throw new ArgumentNullException(nameof (mapper));
    }


    /// <summary>
    /// Get all books
    /// </summary>
    /// <returns>A List of books</returns>
    /// <response code="200">A List of books</response>
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<BookDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
    [HttpGet("get-all-books",Name ="books")]
    public async Task<IActionResult> GetBooks()
    {
        #region ModelState
        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState.GetApiResponse());
        //} 
        #endregion
        try
        {
            _logger.LogInformation("info");
            ServiceResponse<IEnumerable<BookDto>> response = await _booksRepository.GetBooksAsync();


            return response.Code switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.NotFound => NotFound(_mapper.Map<ServiceFailedResponse>(response)),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Conflict => Conflict(response),
                (int)HttpStatusCode.InternalServerError => StatusCode(500, response),

                _ => StatusCode(422, response),
            };


        }
        catch (Exception ex)
        {

            _logger.LogError($"{ex}");
            ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
            return StatusCode(500, serviceResponse);
        }
    }


    /// <summary>
    /// Get all books
    /// </summary>
    /// <returns>A List of books</returns>
    /// <response code="200">A List of books</response>
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<BookDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
    [HttpGet("get-books-by-id", Name = "bookById")]
    public async Task<IActionResult> GetBookById([FromQuery] BooksId booksId)
    {
        #region ModelState

        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState.GetApiResponse());
        //} 
        #endregion
        try
        {
            _logger.LogInformation("info");
            ServiceResponse<BookDto?> response = await _booksRepository.GetBookByIdAsync(booksId.Id);


            return response.Code switch
            {
                (int)HttpStatusCode.OK => Ok(response),
                (int)HttpStatusCode.NotFound => NotFound(_mapper.Map<ServiceFailedResponse>(response)),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Conflict => Conflict(response),
                (int)HttpStatusCode.InternalServerError => StatusCode(500, response),

                _ => StatusCode(422, response),
            };


        }
        catch (Exception ex)
        {

            _logger.LogError($"{ex}");
            ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
            return StatusCode(500, serviceResponse);
        }
    }




    /// <summary>
    /// Create books
    /// </summary>
    /// <returns> book</returns>
    /// <response code="201">Book</response>
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ServiceForbidenResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServiceFailedResponse))]
    [HttpPost()]
    public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto bookforCreationDto)
    {
        #region ModelState

        //if (!ModelState.IsValid)
        //{
        //    return BadRequest(ModelState.GetApiResponse());
        //} 
        #endregion
        try
        {
            _logger.LogInformation("info");


            ServiceResponse<BookDto?> response = await _booksRepository.CreateBookAsync(bookforCreationDto);

            return response.Code switch
            {
                (int)HttpStatusCode.Created => CreatedAtRoute("bookById", new BooksId { Id = response.Data.Id }, response),
                (int)HttpStatusCode.NotFound => NotFound(_mapper.Map<ServiceFailedResponse>(response)),
                (int)HttpStatusCode.BadRequest => BadRequest(response),
                (int)HttpStatusCode.Conflict => Conflict(response),
                (int)HttpStatusCode.InternalServerError => StatusCode(500, response),

                _ => StatusCode(422, response),
            };


        }
        catch (Exception ex)
        {

            _logger.LogError($"{ex}");
            ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
            return StatusCode(500, serviceResponse);
        }
    
    
    
    
    
    }







    [HttpGet("hello-world", Name = "helloworld"), AllowAnonymous]
    [NonProductionActionFilter]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<Testla>))]
    public IActionResult HelloWorld()
    {
        ServiceResponse<Testla> response = new();
        Testla testla = new()
        {
            Result = "Hello world"
        };
        response.Code = 200;
        response.Data = testla;
        response.IsSuccess = true;
        response.Message = "success";



        return Ok(response);
    }

    [HttpGet("{manufacture}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<ProductDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
    public IActionResult Manufacture([FromRoute(Name = "manufacture")][DefaultValue("sony")] string brand, [FromQuery(Name = "coverage")][DefaultValue(2)] int warrantyYears)
    {
        try
        {
            ServiceResponse<ProductDto> response = new();
            
            ProductDto prd = _mapper.Map<ProductDto>(_products.FirstOrDefault(x => x.Brand.Equals(brand, StringComparison.CurrentCultureIgnoreCase) && x.WarrantyYears == warrantyYears));

            if (prd != null)
            {
                response.Data = prd;
                response.Code = 200;
                response.Message= "success";
                return Ok(response);

            }
            ServiceFailedResponse res = _mapper.Map<ServiceFailedResponse>(response);
            res.Message = "product not found";
            res.IsSuccess = false;
            return NotFound(res);
        }
        catch (Exception ex)
        {
            _logger.LogError($"The exception occur at  {ControllerContext.ActionDescriptor.ControllerName} Controller {ControllerContext.ActionDescriptor.ActionName} method -> {ex}");
            ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
            return StatusCode(500, serviceResponse);
        }
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public IActionResult Products()
    {

        try
        {


            if (_products.Count > 0)
                return Ok(new ServiceFailedResponse { Code = 400, Message = "success" });
            return NotFound(new ServiceFailedResponse { Code = 400, Message = "success" });
        }
        catch (Exception ex)
        {

            _logger.LogError($"{ex}");
            ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
            return StatusCode(500, serviceResponse);
        }
    }


    [HttpPatch("UpdateAppointmentRequest/{id:guid}")]
    public async Task<IActionResult> Update(Guid id,
        [FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken cancellationToken = default)
    { 
        ServiceResponse<string> response = new();
        if (isGUID(id))
        {
        response.Data = $"User with header value {ifMatch} created successfully! {id}";
        response.Message= "success";
        return Ok(response);
        }
        else
        {
            response.Data = $"User with header value {ifMatch} wasnt updated";
            response.Message = "failed";
            return BadRequest(response);
        }
        
       
    }

    [NonAction]
    private bool isGUID(Guid str)
    {

        if (Guid.TryParse(str.ToString(), out Guid xCorrelationId))
            return true;
        return false;
    }

}

