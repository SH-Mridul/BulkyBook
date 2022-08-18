using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitofWork _unitOfWork;

        public CategoryController(IUnitofWork db)
        {
            _unitOfWork  = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll().OrderBy(x => x.DisplayOrder);
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
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
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

            Category category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

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
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
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

              Category category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

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
            Category category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

            if (category != null)
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
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
