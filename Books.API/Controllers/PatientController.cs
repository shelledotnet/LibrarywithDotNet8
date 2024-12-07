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
using System.Collections.Generic;

namespace Books.API.Controllers
{
   // [ApiExplorerSettings(IgnoreApi = true)] // Hides from Swagger
    [Produces("application/json", "application/xml")]  //output formatter Media type: Accept header
    [Consumes("application/json")] //input-formatter Media type: content-type header
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ServiceFailedResponse))]
    [ServiceFilter(typeof(RequestAuthActionFilterAttribute))]
    [TypeFilter(typeof(ApiKeyAuthorizationFilterAttribute))]//basicautorization for API
    public class PatientController : ControllerBase
    {
        private readonly EmployeeManagerDbContext _employeeManagerDbContext;

        public PatientController(EmployeeManagerDbContext employeeManagerDbContext)
        {
            _employeeManagerDbContext = employeeManagerDbContext;
        }
        /// <summary>
        /// patient records
        /// </summary>
        /// <param name="registerRequestDto"></param>
        /// <returns></returns>
        [HttpPost("patient-records/import", Name = "books")]
        public async Task<IActionResult> Patient(IFormFile file)
        {
            if(file.Length == 0 || !file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { IsSuccess = false, Message = "invalid file , please upload a non-empty CSV file.", Code = 400 });

            }

            using var reader = new StreamReader(file.OpenReadStream());
            var header=await reader.ReadLineAsync();

            //validate that the header on the file contains required headers
            var headerValues = (header ?? "").Split(",").Select(x => x.Trim());
            string[] requiredFields = ["FirstName", "LastName", "Email", "Address","Phone"];
            if (!requiredFields.All(x => headerValues.Contains(x)))
            {
                return BadRequest(new { IsSuccess = false, Message = "invalid file , Please upload a CSV FILE with the required field", Code = 400 });
            }

            //safe file in a Temp Directory
            Guid jobid=Guid.NewGuid();
            await SaveFileToTempAsync(file,jobid);

            var job = new ImportJob()
            {
                FileName = $"{jobid}.csv",
                DomainId = jobid,
                Status = JobStatus.Enqueued,
                CreatedAt = DateTime.Now
            };
            
            if( await PatientSaveChangesAsync(job))
            {
                return Ok(new {id=jobid,message="File upload successfully. Importing patient records..."});
            }
            return BadRequest(new { id = jobid, message = "issue uploading patient records..." });
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
