using Books.domain.Models;
using Books.Domain.DbContexts;
using Books.Domain.Entities;
using Books.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Books.Domain.Service
{
    public class BooksRepository : IBooksRepository
    {
        private readonly ProjectOptions _projectOptions;
        private readonly ILogger<BooksRepository> _logger;
        private readonly BookContext _bookContext;
        private IClientHeader _clientHeader;

        public BooksRepository(BookContext bookContext, IOptionsMonitor<ProjectOptions> projectOptions,
            ILogger<BooksRepository> logger, IClientHeader clientHeader)
        {
            _bookContext = bookContext ??
                throw new ArgumentNullException(nameof(bookContext));
            _projectOptions = projectOptions.CurrentValue;
            _logger = logger;
            _clientHeader = clientHeader;
        }
        public async Task<ServiceResponse<Book?>> GetBookByIdAsync(Guid id)
        {
            ServiceResponse<Book?> response = new();

            try
            {

                if (!isGUID(id))
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.BadRequestDescription;
                    response.Code = _projectOptions.BadRequest;
                }



                var book  = await _bookContext.Books
                                 .FirstOrDefaultAsync(x => x.Id == id);


                if (book is not null)
                {
                    response.IsSuccess = true;
                    response.Message = _projectOptions.IsSuccess;
                    response.Code = _projectOptions.Ok;
                    response.Data = book;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.NOtFoundDescription;
                    response.Code = _projectOptions.NotFound;
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.IsSuccess = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;

        }

        public async Task<ServiceResponse<IEnumerable<Book>>> GetBooksAsync()
        {
            ServiceResponse<IEnumerable<Book>> response = new();

            string ci = _clientHeader.getclientId();

            string cl = _clientHeader.getcorrelationId();

            try
            {
                List<Book> books = await _bookContext.Books
                                .Include(b => b.Author)
                                .ToListAsync();
                if (books?.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Message = _projectOptions.IsSuccess;
                    response.Code = _projectOptions.Ok;
                    response.Data = books;
                }
                else if(books?.Count <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = _projectOptions.IsFail;
                    response.Code = _projectOptions.NotFound;
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex}";
                _logger.LogError(message);
                response.IsSuccess = false;
                response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
            }
            return response;
        }

        private bool isGUID(Guid str)
        {

            if (Guid.TryParse(str.ToString(), out Guid xCorrelationId))
                return true;
            return false;
        }
    }
}
