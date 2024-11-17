using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [RegularExpression(@"^[-0-9a-zA-Z ]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(150, ErrorMessage = "{0} max Length is 150"), MinLength(3, ErrorMessage = "{0} must be at least 3 characters long")]
        public string? Name { get; set; }

        #region Collections navigation property
        public List<Employee> Employees { get; set; } = new();  //to avoid null reference exceptions 
        #endregion

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
