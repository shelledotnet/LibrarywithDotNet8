﻿// Ignore Spelling: Dto bookfor

using AutoMapper;
using Books.domain.Models;
using Books.Domain.DbContexts;
using Books.Domain.Dto;
using Books.Domain.Entities;
using Books.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Books.Domain.Service;

public class BooksRepository : IBooksRepository 
{
    private readonly ProjectOptions _projectOptions;
    private readonly ILogger<BooksRepository> _logger;
    private readonly BookContext _bookContext;
    private readonly IClientHeader _clientHeader;
    private readonly IMapper _mapper;

    public BooksRepository(BookContext bookContext, IOptionsMonitor<ProjectOptions> projectOptions,
        ILogger<BooksRepository> logger, IClientHeader clientHeader, IMapper mapper)
    {
        _bookContext = bookContext ??
            throw new ArgumentNullException(nameof(bookContext));
        _projectOptions = projectOptions.CurrentValue;
        _logger = logger;
        _clientHeader = clientHeader;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<ServiceResponse<BookDto?>> GetBookByIdAsync(Guid id)
    {
        ServiceResponse<BookDto?> response = new();

        try
        {

            if (!isGUID(id))
            {
                response.IsSuccess = false;
                response.Message = _projectOptions.BadRequestDescription;
                response.Code = HttpStatusCode.BadRequest;
            }



            var book  = await _bookContext.Books
                             .Include(b => b.Author)
                             .FirstOrDefaultAsync(x => x.Id == id);


            if (book is not null)
            {
                response.IsSuccess = true;
                response.Message = _projectOptions.IsSuccess;
                response.Code = HttpStatusCode.OK;
                response.Data = _mapper.Map<BookDto>(book);
            }
            else
            {
                response.IsSuccess = false;
                response.Message = _projectOptions.NOtFoundDescription;
                response.Code = HttpStatusCode.NotFound;
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

    public async Task<ServiceResponse<IEnumerable<BookDto>>> GetBooksAsync()
    {
        ServiceResponse<IEnumerable<BookDto>> response = new();

        string clintId = _clientHeader.getclientId();

        string correlationId = _clientHeader.getcorrelationId();

        _logger.LogInformation($"info- {clintId} - {correlationId}");

        try
        {
            List<Book> books = await _bookContext.Books
                            .Include(b => b.Author)
                            .ToListAsync();
            if (books?.Count > 0) //list could be null though
            {
                response.IsSuccess = true;
                response.Message = _projectOptions.IsSuccess;
                response.Code = HttpStatusCode.OK;
                response.Data = _mapper.Map<List<BookDto>>(books);
            }
            else if(books?.Count <= 0)
            {
                response.IsSuccess = false;
                response.Message = _projectOptions.IsFail;
                response.Code = HttpStatusCode.NotFound;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"GetBooksAsync {ex}");
            response.IsSuccess = false;
            response.Code = HttpStatusCode.InternalServerError;       
            response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
        }
        return response;
    }

   public async Task<ServiceResponse<BookDto?>> CreateBookAsync(BookForCreationDto bookforCreationDto)
    {
        ServiceResponse<BookDto?> response = new();
        try
        {
            if(ValidateInput.ValidateUserInput(bookforCreationDto)) 
            {

                response.IsSuccess = false;
                response.Code = HttpStatusCode.BadRequest;
                response.Message = _projectOptions.BadRequestDescriptioValue;
                return response;


            }

            Book bookEntity = _mapper.Map<Book>(bookforCreationDto);
            AddBook(bookEntity);
            bool add = await SaveChangesAsync();
            if(!add)
            {
                response.IsSuccess = false;
                response.Code = HttpStatusCode.ServiceUnavailable;
                response.Message = _projectOptions.BookFailed;
                return response;
            }
            var book = await _bookContext.Books
                            .Include(b => b.Author)
                            .FirstOrDefaultAsync(x => x.Id == bookEntity.Id);


            if (book is not null)
            {
                response.IsSuccess = true;
                response.Message = _projectOptions.IsSuccess;
                response.Code = HttpStatusCode.Created;
                response.Data = _mapper.Map<BookDto>(book);
            }
            else
            {
                response.IsSuccess = false;
                response.Message = _projectOptions.NOtFoundDescription;
                response.Code = HttpStatusCode.NotFound;
            }


        }
        catch (Exception ex)
        {
            _logger.LogError($"CreateBookAsync {ex}");
            response.Code = HttpStatusCode.FailedDependency;
            response.IsSuccess = false;
            response.Message = ex.InnerException?.Message != null ? ex.InnerException.Message : ex.Message;
        }
        return response;

    }

    private void AddBook(Book bookToAdd)
    {
        try
        {
            if(bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));

            }
            //why using Async here its not an i/o function .. its only been added to entity set not yet persist to db
            _bookContext.Add(bookToAdd);

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task<bool> SaveChangesAsync()
    {
        //this is an i/o function that need async-- that persist to db 
        //return true if 1 or more entites were persisted to db
        return (await _bookContext.SaveChangesAsync() > 0);
    }

    private bool isGUID(Guid str)
    {

        if (Guid.TryParse(str.ToString(), out Guid xCorrelationId))
            return true;
        return false;
    }



   
}
