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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationContext db;
        public ApplicationUserRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }
    }
}
