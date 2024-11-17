using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class Employee
    {   
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [RegularExpression(@"^[-0-9a-zA-Z ]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(150, ErrorMessage = "{0} max Length is 150"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [RegularExpression(@"^[-0-9a-zA-Z ]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(150, ErrorMessage = "{0} max Length is 150"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public bool IsDeveloper { get; set; }

        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        public string? State { get; set; }
        public double Salary { get; set; }
        public string? Gender { get; set; }

        #region Reference Navigation Property
        [Required]
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        #endregion

        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        [Timestamp]
        public byte[]? Timestamp { get; set; }
        //always add  this property for databse update integrity  .. if a user is updating this entity ..
        //notify other users  by adding a catch block for handling optimistic concurrency --> DbUpdateConcurrencyException


        private static int CalculateAges(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }
    }
}
