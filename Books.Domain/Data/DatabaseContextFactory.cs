using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Books.Domain.DbContexts;

namespace Books.Domain.Data
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<BookContext>
    {

        public BookContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookContext>();
            optionsBuilder.UseSqlServer("Data Source=(localDb)\\MSSQLLocalDb;Initial Catalog=BooksDb;");

            return new BookContext(optionsBuilder.Options);
        }
    }
}
