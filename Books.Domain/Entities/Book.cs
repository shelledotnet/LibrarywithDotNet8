using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Books.Domain.Entities;

    [Table("Books")]
    public class Book
    {
        #region Scalar-Property
        [Key]
        public Guid Id { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(150, ErrorMessage = "{0} maxium character length is 150"), MinLength(3, ErrorMessage = "{0} maxium character lenght is 3")]
        public string? Title { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "invalid {0} ")]
        [StringLength(50, ErrorMessage = "{0} maxium character length is 50"), MinLength(3, ErrorMessage = "{0} maxium character lenght is 3")]
        public string? Description { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDdate { get; set; }
        #endregion

        #region Reference-Navigation-Property
        public Guid AuthorId { get; set; }

        public Author? Author { get; set; } = null;

        #endregion


        

        public Book(Guid id, Guid authorId, string? title, string? description)
        { 
            Id = id;
            Title = title;
            Description = description;
            AuthorId = authorId;   

        }

}
