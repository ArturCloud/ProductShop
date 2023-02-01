using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Models.ViewModels;
using ProjectX_Utility;

namespace ProjectX.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository inqDRepos;
        private readonly IInquiryHeaderRepository inqHRepos;

        [BindProperty]
        public InquiryViewModel inquiryVM { get; set; }

        public InquiryController(IInquiryDetailRepository inqDRepos, IInquiryHeaderRepository inqHRepos)
        {
            this.inqDRepos = inqDRepos;
            this.inqHRepos = inqHRepos;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            inquiryVM = new InquiryViewModel()
            {
                InquiryHeaderVM = inqHRepos.FirstOrDefault(x => x.Id == id),
                InquiryDetailVM = inqDRepos.GetAll(u => u.InquiryHeaderId == id, includeProperties: "ProductInquiry") 
            };

            return View(inquiryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details()
        {
            List<ShoppingCart> shoppingCart = new List<ShoppingCart>();

            inquiryVM.InquiryDetailVM = inqDRepos.GetAll(x => x.InquiryHeaderId == inquiryVM.InquiryHeaderVM.Id);

            foreach (var detail in inquiryVM.InquiryDetailVM)
            {
                ShoppingCart cart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                    Sqft = 1
                };
                shoppingCart.Add(cart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstant.SessionCart, shoppingCart);
            HttpContext.Session.Set(WebConstant.SessionInquiryId, inquiryVM.InquiryHeaderVM.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inqH = inqHRepos.FirstOrDefault(o=>o.Id == inquiryVM.InquiryHeaderVM.Id);
            IEnumerable<InquiryDetail> listDetails = inqDRepos.GetAll(o=>o.InquiryHeaderId == inqH.Id);

            inqDRepos.RemoveRange(listDetails);
            inqHRepos.Remove(inqH);
            inqHRepos.Save();
            inqDRepos.Save();

            return RedirectToAction("Index");
        }
        

        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = inqHRepos.GetAll() });
        }
        #endregion
    }
}
