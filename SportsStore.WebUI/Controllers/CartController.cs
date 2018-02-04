using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository repository;
        private IOrderProcessor orderProcessor;

        public CartController(IProductRepository repo, IOrderProcessor proc)
        {
            repository = repo;
            orderProcessor = proc;
        }

        public ViewResult Index(Cart cart, string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }


        public RedirectToRouteResult AddToCart(Cart cart, int productID, string returnUrl)
        {
            var Product = repository.Products.FirstOrDefault(s => s.ProductID == productID);
            if (Product != null)
                cart.AddItem(Product, 1);

            return RedirectToAction("Index", new { returnUrl });

        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int productID, string returnUrl)
        {
            var Product = repository.Products.FirstOrDefault(s => s.ProductID == productID);
            if (Product != null)
                cart.RemoveLine(Product);

            return RedirectToAction("Index", new { returnUrl });
        }

        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingdetails)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Koszyk jest pusty!");
            }

            if (ModelState.IsValid)
            {
                orderProcessor.ProcessOrder(cart, shippingdetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingdetails);
            }
        }
    }
}