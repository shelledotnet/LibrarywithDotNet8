using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    #region InMemmory-Repository-DataSource
    public static class CollegeRepository
    {
        public static List<Student> Student { get; set; } =
        [
             new Student
            {
                Id = 1,
                Age=23,
                Name="Adela"

            },
            new Student
            {
                Id = 2,
                Age=12,
                Name="Mariam"

            }
        ];
    }

    #endregion
}
