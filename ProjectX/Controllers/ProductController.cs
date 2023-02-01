using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using ProjectX_Models;
using ProjectX;

using ProjectX_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ProjectX_Utility;
using ProjectX_Data;
using ProjectX_Data.Repository.IRepository;

namespace ProjectX.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class ProductController : Controller
    {
       private readonly IProductRepository prodRepos;
       private readonly IWebHostEnvironment env;
        public ProductController(IProductRepository prodRepos, IWebHostEnvironment env)
        {
            this.env = env;
            this.prodRepos = prodRepos;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objListProducts = prodRepos.GetAll(includeProperties:"Category");


            return View(objListProducts);
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productVM = new ProductViewModel()
            {
                ProductsVM = new Product(),
                CategoriesVM = prodRepos.GetAllDropDownList()
            };

            if(id == null)
            {
                //creating
                return View(productVM);
            }
            else //editing
            {
                productVM.ProductsVM = prodRepos.Find(id.GetValueOrDefault());

                if (productVM.ProductsVM == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel prodVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;  
                var webPath = env.WebRootPath;  // path to the root file (wwwroot)

                if (prodVM.ProductsVM.Id == 0)    // create
                {
                    var upload = webPath + WebConstant.ImagePath;
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    prodVM.ProductsVM.Image = fileName + extension; 

                    prodRepos.Add(prodVM.ProductsVM);
                }
                else //edit
                {
                    var objOld = prodRepos.FirstOrDefault(o => o.Id == prodVM.ProductsVM.Id, isTracking: false); // find the old product type object; also add AsNoTracking() to solve the problem

                    if (files.Count > 0) // checking for the presence of photo
                    {
                        var upload = webPath + WebConstant.ImagePath; 
                        var fileName = Guid.NewGuid().ToString();
                        var extension = Path.GetExtension(files[0].FileName);


                        var oldFile = Path.Combine(upload, objOld.Image);    // get an old photo, check for existence and delete

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);   
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream); // use a new photo
                        }
                        prodVM.ProductsVM.Image = fileName + extension;
                    }
                    else
                    {
                        prodVM.ProductsVM.Image = objOld.Image;
                    }
                    prodRepos.Update(prodVM.ProductsVM);  // updating
                }
                prodRepos.Save(); // don't forget to save
                return RedirectToAction("Index");
            }

            prodVM.CategoriesVM = prodRepos.GetAllDropDownList();
            return View(prodVM);
        }

        public IActionResult Delete(int? id)
        {
            
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            var obj = prodRepos.FirstOrDefault(i => i.Id == id, includeProperties:"Category");
            //obj.Category = db.Categories.Find(obj.CategoryId);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id) // find the object, find and delete its photo, and then delete the object
        {
            var obj = prodRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            var upload = env.WebRootPath + WebConstant.ImagePath; 
            
            var oldFile = Path.Combine(upload, obj.Image);    

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            prodRepos.Remove(obj);
            prodRepos.Save();

            return RedirectToAction("Index");
        }
    }
}
