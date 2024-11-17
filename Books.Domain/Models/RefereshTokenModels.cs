using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class RefereshTokenModels
    {
        [Key]
        public int Id { get; set; }
        public int UsersId { get; set; }
        public string? JwtId { get; set; }   //jwt id of the refereshToken
        public string? Token { get; set; }  //refereshToken

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateExpired { get; set; }

        [ForeignKey(nameof(UsersId))]
        public Users? Users { get; set; }

    }
}
