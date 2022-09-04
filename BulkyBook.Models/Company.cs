using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required!"),Display(Name ="Name")]
        public string Name { get; set; }
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        [Display(Name = "City")]
        public string? City { get; set; }
        [Display(Name = "State")]
        public string? State { get; set; }
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }
    }
}
