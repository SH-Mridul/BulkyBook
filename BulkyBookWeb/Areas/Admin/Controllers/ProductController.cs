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
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
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
                   
                    if(obj.Product.ImagePath != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImagePath.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads,fileName+extention),FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    obj.Product.ImagePath = @"\Images\Products\" + fileName + extention;
                }

               
                if (obj.Product.Id == null || obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    
                    TempData["success"] = "successfully created!";
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "successfully updated!";
                }

                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        #region Api Calls 
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList});
        }

        //post
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (obj != null)
            {
                if (obj.ImagePath != null)
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImagePath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Product.Remove(obj);
                _unitOfWork.Save();
                return Json(new { success = true,message = "product deleted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "product not found!" });
            }
        }
        #endregion
    }
}
