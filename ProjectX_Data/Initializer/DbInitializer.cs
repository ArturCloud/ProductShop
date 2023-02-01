using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectX_Models;
using ProjectX_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectX_Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbInitializer(ApplicationContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void Initialize()   // in this method, when connecting to a new db, new roles and a new admin will be automatically created
        {
            try
            {
                if (db.Database.GetPendingMigrations().Count() > 0)  // search for incomplete migrations and create them
                {
                    db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!roleManager.RoleExistsAsync(WebConstant.AdminRole).GetAwaiter().GetResult())  // check if our roles are exist and create 
            {
                 roleManager.CreateAsync(new IdentityRole(WebConstant.AdminRole)).GetAwaiter().GetResult();
                 roleManager.CreateAsync(new IdentityRole(WebConstant.CustomerRole)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            userManager.CreateAsync(new ApplicationUser
            {
                UserName = "artur@admin.com",
                Email = "artur@admin.com",
                EmailConfirmed = true,
                FullName = "Admin Tester",
                PhoneNumber = "23423423"
            }, "Admin123.").GetAwaiter().GetResult();

            ApplicationUser appUser = db.ApplicationUsers.FirstOrDefault(u=>u.Email == "artur@admin.com");
            userManager.AddToRoleAsync(appUser, WebConstant.AdminRole).GetAwaiter().GetResult();
        }
    }
}
