using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Models.ViewModels;
using ProjectX_Utility;
using ProjectX_Utility.BrainTree;

namespace ProjectX.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository ordHRepos;
        private readonly IOrderDetailRepository ordDRepos;
        private readonly IBrainTreeGate brainTree;

        [BindProperty]
        public OrderViewModel OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository ordHRepos, IOrderDetailRepository ordDRepos, IBrainTreeGate brainTree)
        {
            this.ordHRepos = ordHRepos;
            this.ordDRepos = ordDRepos;
            this.brainTree = brainTree;
        }

        public IActionResult Index(string searchName=null, string searchEmail=null, string searchPhone=null, string Status=null)
        {
            OrderListViewModel ordListVM = new OrderListViewModel()
            {
                OrderHeaderList = ordHRepos.GetAll(),
                StatusList = WebConstant.listStatus.ToList().Select(i=>new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                ordListVM.OrderHeaderList = ordListVM.OrderHeaderList.Where(u => u.FullName.ToLower() == searchName.ToLower());
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                ordListVM.OrderHeaderList = ordListVM.OrderHeaderList.Where(u => u.Email.ToLower() == searchEmail.ToLower());
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                ordListVM.OrderHeaderList = ordListVM.OrderHeaderList.Where(u => u.PhoneNumber.ToLower() == searchPhone.ToLower());
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                ordListVM.OrderHeaderList = ordListVM.OrderHeaderList.Where(u => u.OrderStatus.ToLower() == Status.ToLower());
            }

            return View(ordListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderViewModel()
            {
                OrderHeaderVM = ordHRepos.FirstOrDefault(o => o.Id == id),
                OrderDetailsVM = ordDRepos.GetAll(u=>u.OrderHeaderId == id,includeProperties:"Products")
            };

            return View(OrderVM);
        }
        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader ordHeader = ordHRepos.FirstOrDefault(U => U.Id == OrderVM.OrderHeaderVM.Id);
            ordHeader.OrderStatus = WebConstant.StatusInProcess;
            ordHRepos.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader ordHeader = ordHRepos.FirstOrDefault(U => U.Id == OrderVM.OrderHeaderVM.Id);
            ordHeader.OrderStatus = WebConstant.StatusShipped;
            ordHeader.ShippingDate = DateTime.Now;
            ordHRepos.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader ordHeader = ordHRepos.FirstOrDefault(U => U.Id == OrderVM.OrderHeaderVM.Id);

            var gateway = brainTree.GetGateway();
            Transaction transaction = gateway.Transaction.Find(OrderVM.OrderHeaderVM.TransactionId);

            if(transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                // no refund
                Result<Transaction> resultVoid = gateway.Transaction.Void(ordHeader.TransactionId);
                
            }
            else
            {
                // refund
                Result<Transaction> resultRefund = gateway.Transaction.Refund(ordHeader.TransactionId);
                
            }
            ordHeader.OrderStatus = WebConstant.StatusRefunded;
            ordHRepos.Save();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader ordHeaderFromDb = ordHRepos.FirstOrDefault(i => i.Id == OrderVM.OrderHeaderVM.Id);
            ordHeaderFromDb.FullName = OrderVM.OrderHeaderVM.FullName;
            ordHeaderFromDb.PhoneNumber = OrderVM.OrderHeaderVM.PhoneNumber;
            ordHeaderFromDb.StreetAddress = OrderVM.OrderHeaderVM.StreetAddress;
            ordHeaderFromDb.City = OrderVM.OrderHeaderVM.City;
            ordHeaderFromDb.State = OrderVM.OrderHeaderVM.State;
            ordHeaderFromDb.PostalCode = OrderVM.OrderHeaderVM.PostalCode;
            ordHeaderFromDb.Email = OrderVM.OrderHeaderVM.Email;
            
            ordHRepos.Save();
            return RedirectToAction("Details", "Order", new {id = ordHeaderFromDb.Id});
        }
    }
}
