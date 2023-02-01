using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using ProjectX_Data;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Models.ViewModels;
using ProjectX_Utility;
using System.Diagnostics;


namespace ProjectX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository prodRepos;
        private readonly ICategoryRepository catRepos;

        public HomeController(ILogger<HomeController> logger, IProductRepository prodRepos, ICategoryRepository catRepos)
        {
            _logger = logger;
            this.prodRepos = prodRepos;
            this.catRepos = catRepos;
        }

        public IActionResult Index()
        {
            HomeViewModel homeVM = new HomeViewModel()
            {
                ProductsHVM = prodRepos.GetAll(includeProperties: "Category"),  //db.Products.Include(u => u.Category),
                CategoriesHVM = catRepos.GetAll()                     //db.Categories
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart>? shopCart = new List<ShoppingCart>(); 

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0) // checking if our session is empty
            {
                shopCart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            DetailsViewModel detailsViewModel = new DetailsViewModel()  
            {
                ProductsDVM = prodRepos.FirstOrDefault(o => o.Id == id, includeProperties:"Category"),           //db.Products.Include(o => o.Category).FirstOrDefault(o => o.Id == id),
                IsInCart = false
            };

            foreach (var item in shopCart)   
            {
                if (item.ProductId == id)
                {
                    detailsViewModel.IsInCart = true;
                }
            }

            return View(detailsViewModel);    
        }
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsViewModel detVM)
        {
            List<ShoppingCart>? shopCart = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0)  
            {
                shopCart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);   
            }

            shopCart.Add(new ShoppingCart { ProductId = id, Sqft = detVM.ProductsDVM.TempSqft });    // add a product to model
            HttpContext.Session.Set(WebConstant.SessionCart, shopCart); // setting data into the session through the model

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart>? shopCart = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0) 
            {
                shopCart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);   
            }
            var obj = shopCart.FirstOrDefault(o => o.ProductId == id);
            if (obj != null)
            {
                shopCart.Remove(obj);
            }

            HttpContext.Session.Set(WebConstant.SessionCart, shopCart);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}