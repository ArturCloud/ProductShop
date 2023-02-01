
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectX_Models
{
    public class Product
    {
        public Product()
        {
            TempSqft = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        [Range(1,int.MaxValue)]
        public double Price{ get; set; }
        
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [NotMapped]     // wont map on DB
        [Range(1, 10000, ErrorMessage = "This value must be great than 0")]
        public int TempSqft { get; set; }

    }
}
