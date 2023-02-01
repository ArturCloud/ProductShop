using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationContext db;
        public OrderDetailRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public void Update(OrderDetail obj)
        {
            db.OrderDetails.Update(obj);
        }

        
    }
}
