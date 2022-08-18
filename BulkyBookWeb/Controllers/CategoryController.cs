using BulkyBook.DataAccess;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Categories.ToList().OrderBy(x => x.DisplayOrder); 
            return View(categories);
        }

        //get
        public IActionResult Create()
        {
            return View();
        }


        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if(category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("customError","The view order can't exactly match the name!");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "category created successfully!";
                return RedirectToAction("Index");
            }

            return View(category);
        }


        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Category category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("customError", "The view order can't exactly match the name!");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "category updated successfully!";
                return RedirectToAction("Index");
            }

            return View(category);
        }


        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Category category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //post
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            Category category = _db.Categories.Find(id);

            if (category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
                TempData["success"] = "category deleted successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }
    }
}
