namespace ProjectX_Models.ViewModels
{
    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            ProductsDVM = new Product();
        }
        public Product ProductsDVM { get; set; }

        public bool IsInCart { get; set; }
    }
}
