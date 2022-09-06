using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer"),Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        public ShoppintCartVM ShoppintCartVM { get; set; }
        public double orderTotal { get; set; }

        public CartController(IUnitofWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppintCartVM = new()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"Product")
            };

            foreach (var cart in ShoppintCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count,cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppintCartVM.CartTotal += (cart.Price * cart.Count);
            }
            return View(ShoppintCartVM);
        }



        public IActionResult Summary()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //ShoppintCartVM = new()
            //{
            //    ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            //};

            //foreach (var cart in ShoppintCartVM.ListCart)
            //{
            //    cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            //    ShoppintCartVM.CartTotal += (cart.Price * cart.Count);
            //}
            //return View(ShoppintCartVM);
            return View();
        }



        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart,1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <=1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity<=50)
            {
                return price;
            }
            else
            {
                if(quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }

        }
    }
}
