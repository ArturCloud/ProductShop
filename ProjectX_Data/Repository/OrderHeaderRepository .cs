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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationContext db;
        public OrderHeaderRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public void Update(OrderHeader obj)
        {
            db.OrderHeaders.Update(obj);
        }
    }
}
