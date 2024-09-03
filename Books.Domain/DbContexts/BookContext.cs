using Books.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.Domain.DbContexts;

//****Note register the BookContext in the IOC container to be use as a service

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
        //seed database with dummy data
        modelBuilder.Entity<Author>().HasData(
           new(Guid.Parse("d28888e9-2ba9-473a-a40f-a38cb54f9b35"), "Ada", "RR Martin"),
           new(Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "Stephen", "Flyin"));

        modelBuilder.Entity<Book>().HasData(
          new(Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),Guid.Parse("d28888e9-2ba9-473a-a40f-a38cb54f9b35"),"The Winds of winter","RR Martin"),
          new(Guid.Parse("abb6fb40-d72f-402b-b07c-47ff00824bad"), Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "Land", "Flyin"));

        base.OnModelCreating(modelBuilder); 

    }
#else
#endif

}

