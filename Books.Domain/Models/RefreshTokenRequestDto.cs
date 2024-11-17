using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Books.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Domain.Models
{
    public class RefreshTokenRequestDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
