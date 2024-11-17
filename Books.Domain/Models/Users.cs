using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        //byte[] this will help us saving binary nformation here
        public byte[] Password { get; set; }

        //salt key to add more security to our password
        public byte[] PasswordKey { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        public bool Active { get; set; }
        public bool Blocked { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Role> Roles { get; set; }

        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDate { get; set; }

        public DateTime? DateExpired { get; set; }

    }
}
