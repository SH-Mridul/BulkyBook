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
    public class CompanyController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
       

        public CompanyController(IUnitofWork db)
        {
            _unitOfWork = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {

            Company company = new();

            if (id == 0 || id == null)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                
                if (company.Id == null || company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    
                    TempData["success"] = "successfully created!";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
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
            var companies = _unitOfWork.Company.GetAll().OrderByDescending(x => x.Id);
            return Json(new { data = companies });
        }

        //post
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

            if (company != null)
            {
                _unitOfWork.Company.Remove(company);
                _unitOfWork.Save();
                return Json(new { success = true,message = "company deleted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "company not found!" });
            }
        }
        #endregion
    }
}
