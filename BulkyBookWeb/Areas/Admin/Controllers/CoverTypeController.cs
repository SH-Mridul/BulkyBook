using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitofWork _unitOfWork;

        public CoverTypeController(IUnitofWork db)
        {
            _unitOfWork = db;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypes = _unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "successfully created new cover type!";
                return RedirectToAction("Index");
            }

            return View(coverType);
        }

        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                TempData["error"] = "id not can be 0!";
                return RedirectToAction("Index");
            }

            CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null)
            {
                TempData["error"] = "Not found!";
                return RedirectToAction("Index");
            }

            return View(coverType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                TempData["success"] = "updated successfully!";
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        public IActionResult Delete(int? id)
        {
            if(id == 0 || id == null)
            {
                TempData["error"] = "id not to be 0!";
                return RedirectToAction("Index");
            }

            CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if(coverType == null)
            {
                TempData["error"] = "Not found!";
                return RedirectToAction("Index");
            }

            return View(coverType);
        }


        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if(coverType != null)
            {
                _unitOfWork.CoverType.Remove(coverType);
                _unitOfWork.Save();
                TempData["success"] = "cover type successfully deleted!";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Not found,enable to delete!";
            return RedirectToAction("Index");
        }
    }
}
