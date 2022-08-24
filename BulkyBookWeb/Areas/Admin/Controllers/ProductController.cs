using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitofWork db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll().OrderBy(x => x.Id);
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),

                Categories = _unitOfWork.Category.GetAll().OrderBy(c => c.DisplayOrder).Select(
                 u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 }),

                CoverTypes = _unitOfWork.CoverType.GetAll().OrderBy(c => c.Id).Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            if (id == 0 || id == null)
            {
               
                return View(productVM);
            }
            else
            {
                //update
            }

            return View(productVM);
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"Images\Products");
                    var extention = Path.GetExtension(file.FileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads,fileName+extention),FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.Product.ImagePath = @"\Images\Products\" + fileName + extention;
                }

                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
            }

            TempData["success"] = "successfully created!";
            return RedirectToAction("Index");
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
        [HttpPost, ActionName("Delete")]
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

        #region Api Calls 
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll();
            return Json(new { data = productList});
        }
        #endregion
    }
}
