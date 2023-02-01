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
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationContext db;
        public InquiryHeaderRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public void Update(InquiryHeader obj)
        {
            db.InquiryHeaders.Update(obj);
        }
    }
}
