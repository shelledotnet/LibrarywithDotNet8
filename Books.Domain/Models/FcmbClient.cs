using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class FcmbClient
    {

        /// <summary>
        /// client id is required to call this endpoint
        /// </summary>
        [FromHeader]
        //[Required(ErrorMessage = "client id  is required")]
        public string? client_id { get; set; }


        [FromHeader]
        //[Required(ErrorMessage = "correlation id  is required")]
        public string? CorrelationId { get; set; }

        [FromHeader]
        public string? Product_Id { get; set; }
    }
}
