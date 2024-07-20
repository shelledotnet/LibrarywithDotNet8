using Books.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.API.DbContexts;

    public class BookContext : DbContext
    {
    public DbSet<Book> Books { get; set; } = null;

    public BookContext(DbContextOptions<BookContext> options)
        :base(options)
    {

    }


#if DEBUG
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {





        base.OnModelCreating(modelBuilder);
    }
#else
#endif

}

