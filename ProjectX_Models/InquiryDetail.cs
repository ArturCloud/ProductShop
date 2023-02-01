using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Models
{
    public class InquiryDetail
    {
        [Key]
        public int Id { get; set; }
        public int InquiryHeaderId { get; set; }
        [ForeignKey("InquiryHeaderId")]
        public InquiryHeader? InquiryHeader { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? ProductInquiry { get; set; }
    }
}
