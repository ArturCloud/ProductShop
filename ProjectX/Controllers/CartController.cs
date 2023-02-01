using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX_Data;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Models.ViewModels;
using ProjectX_Utility;
using ProjectX_Utility.BrainTree;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;

namespace ProjectX.Controllers
{
    [Authorize]   // you must be logged in to enter the shopping cart
    public class CartController : Controller
    {
        //private readonly ApplicationContext db;

        private readonly IApplicationUserRepository appUserRepos;  
        private readonly IProductRepository prodRepos;
        private readonly IWebHostEnvironment env;
        private readonly IEmailSender emailSender;
        private readonly IInquiryHeaderRepository inqHRepos;
        private readonly IInquiryDetailRepository inqDRepos;
        private readonly IOrderHeaderRepository ordHRepos;
        private readonly IOrderDetailRepository ordDRepos;
        private readonly IBrainTreeGate brainTree;



       [BindProperty]   
        public ProductUserViewModel ProductUserVM { get; set; }
        public CartController(IProductRepository prodRepos, IWebHostEnvironment env, IEmailSender emailSender, IApplicationUserRepository appUserRepos,
           IInquiryHeaderRepository inqHRepos, IInquiryDetailRepository inqDRepos, IOrderHeaderRepository ordHRepos, IOrderDetailRepository ordDRepos, IBrainTreeGate brainTree)
        {
            this.prodRepos = prodRepos;
            this.env = env;
            this.emailSender = emailSender; 
            this.appUserRepos = appUserRepos;
            this.inqHRepos = inqHRepos;
            this.inqDRepos = inqDRepos;
            this.ordDRepos = ordDRepos;
            this.ordHRepos = ordHRepos;
            this.brainTree = brainTree;
        }

        public IActionResult Delete(int id)
        {
            List<ShoppingCart> cart = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0)
            {
                cart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            var obj = cart.FirstOrDefault(u => u.ProductId == id);
            cart.Remove(obj);
            HttpContext.Session.Set<IEnumerable<ShoppingCart>>(WebConstant.SessionCart, cart);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Index()
        {
            List<ShoppingCart> cart = new List<ShoppingCart>(); // create a list of class ShoppingCart
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0)
            {
                cart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart); // move list from session to variable
            }

            List<int> prodInCart = cart.Select(u => u.ProductId).ToList();
            IEnumerable<Product> prodListTemp = prodRepos.GetAll(o => prodInCart.Contains(o.Id));    //db.Products.Where(o => prodInCart.Contains(o.Id));
            IList<Product> prodList = new List<Product>();

         
            foreach (var obj in cart)
            {
                Product prodTemp = prodListTemp.FirstOrDefault(u => u.Id == obj.ProductId);
                prodTemp.TempSqft = obj.Sqft;
                prodList.Add(prodTemp);
            }

            return View(prodList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]

        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WebConstant.AdminRole))
            {
                if(HttpContext.Session.Get<int>(WebConstant.SessionInquiryId) != 0)
                {
                    InquiryHeader inqHeader = inqHRepos.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstant.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inqHeader.Email,
                        FullName = inqHeader.FullName,
                        PhoneNumber = inqHeader.PhoneNumber
                    };
                }
                else
                {
                     applicationUser = new ApplicationUser();
                }

                var gateway = brainTree.GetGateway();
                var clientToken = gateway.ClientToken.Generate();
                ViewBag.ClientToken = clientToken;  

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity?)User.Identity;   // we need to get the user's email, phone and other data \/ this is the first way
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //var userId = User.FindFirstValue(ClaimTypes.Name);  // second way

                applicationUser = appUserRepos.FirstOrDefault(u => u.Id == claim.Value);
            }

            

            List<ShoppingCart> cart = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstant.SessionCart)?.Count() > 0)
            {
                // сессия существует 
                cart = HttpContext.Session.Get<List<ShoppingCart>>(WebConstant.SessionCart);
            }

            List<int> prodInCart = cart.Select(u => u.ProductId).ToList();
            IEnumerable<Product> prodList = prodRepos.GetAll(o => prodInCart.Contains(o.Id)); //db.Products.Where(o => prodInCart.Contains(o.Id));  
            IList<Product> prodListToSent = new List<Product>();

            foreach (var obj in cart)
            {
                Product prod = prodList.FirstOrDefault(u => u.Id == obj.ProductId);
                prod.TempSqft = obj.Sqft;
            }

            ProductUserVM = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser,      //db.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()  // adding ToList due to a problem
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPostAsync(IFormCollection collection,ProductUserViewModel productUserVM)
        {

            var claimIdentity = User.Identity as ClaimsIdentity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);  

            if (User.IsInRole(WebConstant.AdminRole))
            {
                OrderHeader ordHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    TotalFinalOrder = productUserVM.ProductList.Sum(x=>x.TempSqft * x.Price), // instead of foreach we use LINQ
                    City = productUserVM.ApplicationUser.City,
                    StreetAddress = productUserVM.ApplicationUser.StreetAddress,
                    State = productUserVM.ApplicationUser.State,
                    PostalCode = productUserVM.ApplicationUser.PostalCode,
                    FullName = productUserVM.ApplicationUser.FullName,
                    Email = productUserVM.ApplicationUser.Email,
                    PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstant.StatusPending
                };
                ordHRepos.Add(ordHeader);
                ordHRepos.Save();

                foreach (var product in productUserVM.ProductList)
                {
                    OrderDetail ordDetail = new OrderDetail()
                    {
                        OrderHeaderId = ordHeader.Id,
                        PricePerSqft = product.Price,
                        ProductId = product.Id,
                        Sqft = product.TempSqft
                    };
                    ordDRepos.Add(ordDetail);
                }
                ordDRepos.Save();

                string nonceFromTheClient = collection["payment_method_nonce"];
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(ordHeader.TotalFinalOrder),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = ordHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = brainTree.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if(result.Target.ProcessorResponseText == "Approved")
                {
                    ordHeader.TransactionId = result.Target.Id;
                    ordHeader.OrderStatus = WebConstant.StatusApproved;
                }
                else
                {
                    ordHeader.OrderStatus = WebConstant.StatusCancelled;
                }
                ordHRepos.Save();

                return RedirectToAction(nameof(InquiryConfirmation), new {id = ordHeader.Id});
            }
            else
            {
                //creating an inquiry

                var pathToTemplate = env.WebRootPath + Path.DirectorySeparatorChar.ToString() +   // find the path to the template
                "template" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
                var subject = "New Subject";    // new title
                var htmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))  // create a thread in which we pass all the template data to a variable
                {
                    htmlBody = sr.ReadToEnd();   
                }

                // Name: {1}                
                // Email: {2}
                // Phone: {3}
                // Products: {4}

                StringBuilder productListSB = new StringBuilder();

                foreach (var prod in ProductUserVM.ProductList)       // call the stringBuilder, write down the name and id of each product as a string there
                {
                    productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
                }

                var messageBody = string.Format(htmlBody,         // create an html body in string format, which will have 5 parameters
                    productUserVM.ApplicationUser.FullName,          // user data
                    productUserVM.ApplicationUser.Email,
                    productUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());      // products

                await emailSender.SendEmailAsync(WebConstant.MainEmail, subject, messageBody);         //

                InquiryHeader inquiryHeader = new InquiryHeader()  // save the data of the user 
                {
                    ApplicationUserId = claim.Value,        // assign a value from claim.value
                    FullName = productUserVM.ApplicationUser.FullName,
                    Email = productUserVM.ApplicationUser.Email,
                    PhoneNumber = productUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };

                inqHRepos.Add(inquiryHeader);
                inqHRepos.Save();


                foreach (var obj in productUserVM.ProductList)   // save the products 
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = obj.Id,
                    };
                    inqDRepos.Add(inquiryDetail);

                }
                inqDRepos.Save(); //
            }

            return RedirectToAction(nameof(InquiryConfirmation));               
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = ordHRepos.FirstOrDefault(h => h.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }


        [HttpPost, ActionName("UpdateCart")]
        [ValidateAntiForgeryToken]
        public IActionResult Update(IEnumerable<Product> productList)
        {
            List<ShoppingCart> cart = new List<ShoppingCart>();

            foreach (var product in productList)
            {
                cart.Add(new ShoppingCart() {ProductId = product.Id, Sqft = product.TempSqft });
            }

            HttpContext.Session.Set(WebConstant.SessionCart, cart);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}
