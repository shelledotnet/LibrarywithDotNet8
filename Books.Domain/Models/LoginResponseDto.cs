using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class LoginResponseDto
    {
        public string? Username { get; set; }
        public List<string>? Role { get; set; }
        public string? Token { get; set; }

    }
}
