using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Name of Category"), Required(ErrorMessage = "Enter category name!")]
        public string Name { get; set; }

        [DisplayName("Display Order"), Range(1, int.MaxValue, ErrorMessage = "please check the value again!")]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}
