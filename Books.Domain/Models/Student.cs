using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
  public  class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int Age { get; set; }


    }
    public class StudentResponseDto
    {
        public string? Name { get; set; }

        public int Age { get; set; }


    }

    public class StudentForCreation
    {
        [Required]
        public string? Name { get; set; }

        [Range(18, 80,ErrorMessage = "{0} must be between 18 and 80")]
        public int Age { get; set; }


    }
}
