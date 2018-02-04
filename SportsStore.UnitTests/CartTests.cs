using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Entities;
using System.Linq;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void CanAddNewLines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100 };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 200 };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 2);
            target.AddItem(p2, 3);

            Assert.AreEqual(target.Lines.Count(), 2);
            Assert.AreEqual(target.Lines.ToArray()[0].Quantity, 1);
            Assert.AreEqual(target.Lines.ToArray()[1].Quantity, 5);
            Assert.AreEqual(target.ComputeTotalValue(), 1100m);
        }

        [TestMethod]
        public void CanRemoveLines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p3, 3);
            target.AddItem(p2, 2);
            target.AddItem(p3, 3);

            target.RemoveLine(p3);

            var result = target.Lines.ToArray();

            Assert.AreEqual(result.Count(), 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[0].Quantity, 1);
            Assert.AreEqual(result[1].Product, p2);
            Assert.AreEqual(result[1].Quantity, 2);
        }

        [TestMethod]
        public void CanClearContents()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p3, 3);
            target.AddItem(p2, 2);
            target.AddItem(p3, 3);

            target.Clear();

            var result = target.Lines.ToArray();

            Assert.AreEqual(result.Count(), 0);
        }

        [TestMethod]
        public void CanAddToCart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jab"},
            }
            .AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object);

            target.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void AddingProductToCartGoesToCartScreen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Jabłka"},
            }
            .AsQueryable());

            Cart cart = new Cart();
            CartController target = new CartController(mock.Object);

            var result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }


        [TestMethod]
        public void CanViewCartContents()
        {
            Cart cart = new Cart();

            var target = new CartController(null);

            var result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }
    }
}
