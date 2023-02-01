using Microsoft.AspNetCore.Mvc.Rendering;


namespace ProjectX_Models.ViewModels
{
    public class ProductViewModel
    {
        public Product ProductsVM { get; set; }
        
        public IEnumerable<SelectListItem>? CategoriesVM { get; set; } 
    }
}
