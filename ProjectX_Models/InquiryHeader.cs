using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }       // type is string because id is type Guid

        [ForeignKey("ApplicationUserId")]  // connection
        public ApplicationUser? ApplicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}
