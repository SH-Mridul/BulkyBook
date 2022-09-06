using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofWork _unitofWork;

        public HomeController(ILogger<HomeController> logger, IUnitofWork unitOfWork)
        {
            _logger = logger;
            _unitofWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitofWork.Product.GetAll(includeProperties:"Category,CoverType");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitofWork.Product.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,CoverType")
            };

          return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity =(ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            ShoppingCart cartFromDb = _unitofWork.ShoppingCart.GetFirstOrDefault(u=> u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);
            if(cartFromDb == null)
            {
                _unitofWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitofWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            }
            
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}