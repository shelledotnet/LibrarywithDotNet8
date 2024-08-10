using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Models;

public class Product
{
    public int Id { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public string? Name { get; set; }
    public int WarrantyYears { get; set; }
    public bool IsAvailable { get; set; }
}
public class ProductDto
{
    public int Id { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public string? Name { get; set; }
    public int WarrantyYears { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProductViewModel
{
    [FromQuery(Name = "type")]
    [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Invalid {0}")]
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(10, ErrorMessage = "{0} max Length is 20"), MinLength(5, ErrorMessage = "{0} min lenght is 5")]
    public string? Category { get; set; }

    [FromQuery(Name = "manufacturer")]
    [RegularExpression(@"^[0-9a-zA-Z]+$", ErrorMessage = "Invalid {0}")]
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(10, ErrorMessage = "{0} max Length is 20"), MinLength(5, ErrorMessage = "{0} min lenght is 5")]
    public string? Brand { get; set; }
}
