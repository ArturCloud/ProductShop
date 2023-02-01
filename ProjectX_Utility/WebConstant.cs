using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.ObjectModel;

namespace ProjectX_Utility
{
    public static class WebConstant
    {
        public static string ImagePath = @"\images\product\"; // a single path to folder with photos
        public static string SessionCart = "ShoppingCartSession"; // shoppingCart session key
        public static string SessionInquiryId = "InquirySession"; // inquiry session key

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string MainEmail = "lans.grimmo@gmail.com";

        public const string Category = "Category";

        public const string Success = "Success";   // constants for toastr
        public const string Error = "Error";

        public const string StatusPending = "Pending";   //statuses
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";   
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";   
        public const string StatusRefunded = "Refunded";

        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(   // listing all status
            new List<string>
            {
                StatusApproved, StatusPending, StatusInProcess, StatusShipped, StatusCancelled, StatusRefunded
            });
        
           
        
    }
}
