using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Models.ViewModels
{
    public class InquiryViewModel
    {
        public InquiryHeader InquiryHeaderVM { get; set; }
        public IEnumerable<InquiryDetail> InquiryDetailVM { get; set; }
    }
}
