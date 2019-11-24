using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeMugApi.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}