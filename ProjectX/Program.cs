using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using ProjectX_Data;
using ProjectX_Data.Initializer;
using ProjectX_Data.Repository;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Utility;
using ProjectX_Utility.BrainTree;
using System.Configuration;
using System.Drawing.Text;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(connection));


builder.Services.AddIdentity<IdentityUser, IdentityRole>()//  and user and role identity -> AddDefaultIdentity(IdentityUser) меняется на след.
    .AddDefaultTokenProviders().AddDefaultUI()      // add token and UI
    .AddEntityFrameworkStores<ApplicationContext>();

builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("Braintree"));

builder.Services.AddTransient<IEmailSender, EmailSender>();    // email class injection; transient - when requested, a new object is created

builder.Services.AddSingleton<IBrainTreeGate, BrainTreeGate>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();  // keeps the service active for 1 request
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
builder.Services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();

builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.AppId = "551085860285451";
    opt.AppSecret = "34a93503b601be136f996615a2f574d8";
});

builder.Services.AddHttpContextAccessor(); // for session
builder.Services.AddSession(opt =>            // add session with options
{
    opt.IdleTimeout = TimeSpan.FromMinutes(10);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var serviceScope = app.Services.CreateScope())   // to add an Initializer we have to write this code
{
    var services = serviceScope.ServiceProvider;

    var myDependency = services.GetRequiredService<IDbInitializer>();

    //Use the service
    myDependency.Initialize();

}
app.UseSession();
app.MapRazorPages();   // is need for .cshtml.cs files

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
