using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>   
    {
        void Update(Product obj);

        IEnumerable<SelectListItem> GetAllDropDownList();
    }
}
