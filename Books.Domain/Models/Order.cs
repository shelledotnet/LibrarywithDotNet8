using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Books.Domain.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public string? Currency { get; set; }
        public int UsersId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UsersId))]
        public Users? Users { get; set; }
        //the JsonIgnore Attribute to the users object in order to hide it when doing Json serialization

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}
