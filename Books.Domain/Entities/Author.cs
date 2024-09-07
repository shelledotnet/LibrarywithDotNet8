using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Domain.Entities
{

    [Table("Authors")]
    public class Author
    {
        [Key]   
        public Guid Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid requestId ")]
        [StringLength(50, ErrorMessage = "{0} maxium character length is 50"), MinLength(3, ErrorMessage = "{0} maxium character lenght is 3")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid requestId ")]
        [StringLength(50, ErrorMessage = "{0} maxium character length is 50"), MinLength(3, ErrorMessage = "{0} maxium character lenght is 3")]
        public string? LastName { get; set; }


        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime CreatedDdate { get; set; }


        public Author(Guid id,string? firstName,string? lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
