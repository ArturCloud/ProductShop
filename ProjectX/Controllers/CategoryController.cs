using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX_Data;
using ProjectX_Data.Repository.IRepository;
using ProjectX_Models;
using ProjectX_Utility;
using System.Data;


namespace ProjectX.Controllers
{
    [Authorize(Roles = WebConstant.AdminRole)]
    public class CategoryController : Controller
    {
        // ApplicationContext db;
        private readonly ICategoryRepository catRepos;
        public CategoryController(ICategoryRepository catRepos)
        {
            this.catRepos = catRepos;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objectList = catRepos.GetAll();

            return View(objectList);
        }

        //get - Create
        public IActionResult Create()
        {
            return View();
        }

        //post - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category cat)
        {
            if (ModelState.IsValid)
            {
                catRepos.Add(cat);
                catRepos.Save();
                TempData[WebConstant.Success] = "Category created successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstant.Error] = "Error while createing category";
            return View();
        }
        //get - Edit
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var obj = catRepos.Find(id.GetValueOrDefault());  
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                catRepos.Update(obj);
                catRepos.Save();
                TempData[WebConstant.Success] = "Category was edited successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstant.Error] = "Error";
            return View();
        }
        //get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var obj = catRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = catRepos.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            catRepos.Remove(obj);
            catRepos.Save();
            TempData[WebConstant.Success] = "Category was successfully deleted";
            return RedirectToAction("Index");
        }
    }
}
