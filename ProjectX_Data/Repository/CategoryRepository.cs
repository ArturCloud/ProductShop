using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationContext db;
        public CategoryRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public void Update(Category obj)
        {
            var objUpdate = db.Categories.FirstOrDefault(u => u.Id == obj.Id);
            if(objUpdate != null)
            {
                objUpdate.Name = obj.Name;
                objUpdate.DisplayOrder = obj.DisplayOrder;
            }
        }
    }
}
