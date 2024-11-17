using Books.Domain.Entities;
using Books.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Data
{
    //**Note register the EmployeeManagerDbContext in the IOC container to be use as a service
    public class EmployeeManagerDbContext : DbContext
    {
        public EmployeeManagerDbContext(DbContextOptions<EmployeeManagerDbContext> options)
        : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Users> Users => Set<Users>();
        public DbSet<Department> Departments => Set<Department>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Role> Role => Set<Role>();
        public DbSet<RefereshTokenModels> RefereshTokens => Set<RefereshTokenModels>();


#if DEBUG
        #region Seed Data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Finance" },
                 new Department { Id = 2, Name = "IT" },
                 new Department { Id = 3, Name = "Control" },
                  new Department { Id = 4, Name = "HR" },
                   new Department { Id = 5, Name = "Marketing" }
                );
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Adeola", LastName = "Malian", IsDeveloper = false, DepartmentId = 2, Gender = "Male", State = "Ogun", Salary = 61000 },
                new Employee { Id = 2, FirstName = "Olaolu", LastName = "Mapayi", IsDeveloper = true, DepartmentId = 3, Gender = "FeMale", State = "Lagos", Salary = 3400 },
                new Employee { Id = 3, FirstName = "Deji", LastName = "Henry", IsDeveloper = false, DepartmentId = 4, Gender = "FeMale", State = "Oyo", Salary = 8900 },
                new Employee { Id = 4, FirstName = "Sheyi", LastName = "Alao", IsDeveloper = true, DepartmentId = 1, Gender = "Male", State = "Lagos", Salary = 7890 },
                new Employee { Id = 5, FirstName = "Opeola", LastName = "Bayo", IsDeveloper = false, DepartmentId = 5, Gender = "FeMale", State = "Oyo", Salary = 8600 },
                new Employee { Id = 6, FirstName = "Adeola", LastName = "Malian", IsDeveloper = false, DepartmentId = 2, Gender = "Male", State = "Ogun", Salary = 61000 },
                new Employee { Id = 7, FirstName = "Olaolu", LastName = "Mapayi", IsDeveloper = true, DepartmentId = 3, Gender = "FeMale", State = "Lagos", Salary = 3400 },
                new Employee { Id = 8, FirstName = "Deji", LastName = "Henry", IsDeveloper = false, DepartmentId = 4, Gender = "FeMale", State = "Oyo", Salary = 9900 },
                new Employee { Id = 9, FirstName = "Sheyi", LastName = "Alao", IsDeveloper = true, DepartmentId = 1, Gender = "Male", State = "Lagos", Salary = 7890 },
                new Employee { Id = 10, FirstName = "Opeola", LastName = "Bayo", IsDeveloper = false, DepartmentId = 5, Gender = "FeMale", State = "Oyo", Salary = 9600 },
                 new Employee { Id = 11, FirstName = "Adeola", LastName = "Malian", IsDeveloper = false, DepartmentId = 2, Gender = "Male", State = "Ogun", Salary = 61000 },
                new Employee { Id = 12, FirstName = "Olaolu", LastName = "Mapayi", IsDeveloper = true, DepartmentId = 3, Gender = "FeMale", State = "Lagos", Salary = 3400 },
                new Employee { Id = 13, FirstName = "Deji", LastName = "Henry", IsDeveloper = false, DepartmentId = 4, Gender = "FeMale", State = "Oyo", Salary = 8900 },
                new Employee { Id = 14, FirstName = "Sheyi", LastName = "Alao", IsDeveloper = true, DepartmentId = 1, Gender = "Male", State = "Lagos", Salary = 7890 },
                new Employee { Id = 15, FirstName = "Opeola", LastName = "Bayo", IsDeveloper = false, DepartmentId = 5, Gender = "FeMale", State = "Oyo", Salary = 8600 },
                new Employee { Id = 16, FirstName = "Adeola", LastName = "Malian", IsDeveloper = false, DepartmentId = 2, Gender = "Male", State = "Ogun", Salary = 61000 },
                new Employee { Id = 17, FirstName = "Olaolu", LastName = "Mapayi", IsDeveloper = true, DepartmentId = 3, Gender = "FeMale", State = "Lagos", Salary = 3400 },
                new Employee { Id = 18, FirstName = "Deji", LastName = "Henry", IsDeveloper = false, DepartmentId = 4, Gender = "FeMale", State = "Oyo", Salary = 9900 },
                new Employee { Id = 19, FirstName = "Sheyi", LastName = "Alao", IsDeveloper = true, DepartmentId = 1, Gender = "Male", State = "Lagos", Salary = 7890 },
                new Employee { Id = 20, FirstName = "Opeola", LastName = "Bayo", IsDeveloper = false, DepartmentId = 5, Gender = "FeMale", State = "Oyo", Salary = 9600 }




                );

        }
        #endregion
#else
#endif


    }
}
