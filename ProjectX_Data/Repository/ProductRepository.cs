using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationContext db;
        public ProductRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropDownList()
        {
            return db.Categories.Select(s=>new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
        }

        public void Update(Product obj)
        {
            db.Products.Update(obj);
        }

    }
}
