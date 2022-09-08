using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer"),Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofWork _unitOfWork;
        [BindProperty]
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
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppintCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count,cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppintCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppintCartVM);
        }



        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppintCartVM = new()
            {
                ListCart    = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppintCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppintCartVM.OrderHeader.Name = ShoppintCartVM.OrderHeader.ApplicationUser.Name;
            ShoppintCartVM.OrderHeader.PhoneNumber = ShoppintCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppintCartVM.OrderHeader.State = ShoppintCartVM.OrderHeader.ApplicationUser.State;
            ShoppintCartVM.OrderHeader.StreetAddress = ShoppintCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppintCartVM.OrderHeader.City = ShoppintCartVM.OrderHeader.ApplicationUser.City;
            ShoppintCartVM.OrderHeader.PostalCode = ShoppintCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppintCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppintCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppintCartVM);
        }

        [HttpPost,ValidateAntiForgeryToken,ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppintCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");
            ShoppintCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppintCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppintCartVM.OrderHeader.DateTime = System.DateTime.Now;
            ShoppintCartVM.OrderHeader.ApplicationUserId = claim.Value;


            foreach (var cart in ShoppintCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppintCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            _unitOfWork.OrderHeader.Add(ShoppintCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppintCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppintCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppintCartVM.ListCart);
            _unitOfWork.Save();
            return RedirectToAction("Index","Home");
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
