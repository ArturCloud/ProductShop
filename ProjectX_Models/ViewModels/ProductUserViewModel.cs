namespace ProjectX_Models.ViewModels
{
    public class ProductUserViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }   // IEnumerable --> IList due to the problem with foreach -> for
    }
}
