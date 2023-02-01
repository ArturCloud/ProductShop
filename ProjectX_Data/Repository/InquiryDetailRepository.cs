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
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationContext db;
        public InquiryDetailRepository(ApplicationContext db) : base(db)  
        {
            this.db = db;
        }

        public void Update(InquiryDetail obj)
        {
            db.InquiryDetail.Update(obj);
        }

    }
}
