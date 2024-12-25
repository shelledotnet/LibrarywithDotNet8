using Books.API.BackgroundJobs;
using Books.API.Extensions;
using Books.API.Filter;
using Books.domain.Models;
using Books.Domain.Data;
using Books.Domain.DbContexts;
using Books.Domain.Entities;
using Books.Domain.Migrations;
using Books.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;

namespace Books.API.Controllers
{
   // [ApiExplorerSettings(IgnoreApi = true)] // Hides from Swagger
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json", "multipart/form-data")] //input-formatter Media type: content-type header
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [ServiceFilter(typeof(RequestAuthActionFilterAttribute))]
    [TypeFilter(typeof(ApiKeyAuthorizationFilterAttribute))]//basicautorization for API
    public class patientController : ControllerBase
    {
        private readonly EmployeeManagerDbContext _employeeManagerDbContext;
        private readonly ILogger<patientController> _logger;

        public patientController(EmployeeManagerDbContext employeeManagerDbContext, ILogger<patientController> logger)
        {
            _employeeManagerDbContext = employeeManagerDbContext;
            _logger = logger;
        }


        /// <summary>
        /// patient records
        /// </summary>
        /// <param name="registerRequestDto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [HttpPost("records/import", Name = "patient")]
        public async Task<IActionResult> Patient(IFormFile file)
        {
            try
            {
                if (file.Length == 0 || !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new ServiceBadResponse { Message = "invalid file , please upload a non-empty CSV file." });

                }

                using var reader = new StreamReader(file.OpenReadStream());
                var header = await reader.ReadLineAsync();

                //validate that the header on the file contains required headers
                var headerValues = (header ?? "").Split(",").Select(x => x.Trim());
                string[] requiredFields = ["FirstName", "LastName", "Email", "Address", "Phone"];
                if (!requiredFields.All(x => headerValues.Contains(x)))
                {
                    return BadRequest(new ServiceBadResponse { Message = "invalid file , Please upload a CSV FILE with the required field" });



                }

                //safe file in a Temp Directory
                Guid jobid = Guid.NewGuid();
                await SaveFileToTempAsync(file, jobid);

                var job = new ImportJob()
                {
                    FileName = $"{jobid}.csv",
                    DomainId = jobid,
                    Status = JobStatus.Enqueued,
                    CreatedAt = DateTime.Now
                };

                if (await PatientSaveChangesAsync(job))
                {
                    ServiceResponse<string> serviceResponse = new()
                    {
                        Code = System.Net.HttpStatusCode.OK,
                        Message = "File upload successfully. Importing patient records...",
                        Data = jobid.ToString()

                    };
                    return Ok(serviceResponse);
                }
                return BadRequest(new ServiceBadResponse { Message = "issue uploading record" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"PatientController: {ex}");
                ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }


        /// <summary>
        ///get patient records
        /// </summary>ServiceFailedResponse
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ServiceBadResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ServiceFailedResponse))]
        [HttpGet("records/import/{id:guid}", Name = "getpatient")]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            try
            {
               
            var domainId = await  _employeeManagerDbContext.ImportJobs.FirstOrDefaultAsync(job=>job.DomainId.Equals(id));
                if (domainId != null)
                {
                    ServiceResponse<dynamic> serviceResponse = new()
                    {
                        Code = System.Net.HttpStatusCode.OK,
                        Message = "successful",
                        Data = new
                        {
                            status=domainId.Status.ToString(),
                            createdAt=domainId.CreatedAt,
                            startedAt=domainId.StartedAt,
                            updatedAt=domainId.CompletedAt ?? domainId.FailedAt,
                            notes=domainId.FailureReason?.ToString() != null ? "error uploading patient records, Please contact support " : "patient records were uploaded successfully"
                        }

                    };
                    return Ok(serviceResponse);
                }
                return NotFound(new ServiceFailedResponse {Code=(int)HttpStatusCode.NotFound, Message = "id not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPatient: {ex}");
                ServiceFailedResponse serviceResponse = new() { IsSuccess = false, Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message };
                return StatusCode(500, serviceResponse);
            }
        }









        [NonAction]
        private async Task<bool> PatientSaveChangesAsync(ImportJob job)
        {
            try
            {
                if (job == null)
                {
                    throw new ArgumentNullException(nameof(job));

                }
                //why using Async here its not an i/o function .. its only been added to entity set not yet persist to db
                _employeeManagerDbContext.Add(job);

                //this is an i/o function that need async-- that persist to db 
                //return true if 1 or more entites were persisted to db
                return (await _employeeManagerDbContext.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {

                return false;
            }

            
        }

        [NonAction]
        private static async Task SaveFileToTempAsync(IFormFile file, Guid jobid)
        {
            if (!Directory.Exists("temp"))
            {
                Directory.CreateDirectory("temp");
            }
            var tempFilePath = Path.Combine("temp", $"{jobid}.csv");
            await using var filestream = new FileStream(tempFilePath,FileMode.Create,FileAccess.Write);
            await file.OpenReadStream().CopyToAsync(filestream);
        }
    }
}
