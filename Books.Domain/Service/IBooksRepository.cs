using Books.domain.Models;
using Books.Domain.Dto;
using Books.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service;

 public interface IBooksRepository
 {
    Task<ServiceResponse<IEnumerable<BookDto>>> GetBooksAsync();
    Task<ServiceResponse<BookDto?>> GetBookByIdAsync(Guid id);

    void  AddBook(Book bookToAdd);

    Task<bool> SaveChangesAsync();
 }

