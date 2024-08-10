using Books.domain.Models;
using Books.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service;

 public interface IBooksRepository
 {
    Task<ServiceResponse<IEnumerable<Book>>> GetBooksAsync();
    Task<ServiceResponse<Book?>> GetBookByIdAsync(Guid id);
 }

