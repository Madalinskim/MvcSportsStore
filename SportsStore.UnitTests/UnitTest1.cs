using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Collections.Generic;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using System.Linq;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void CanGeneratePageLinks()
        {
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Strona" + i;

            //------------------------------

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //------------------------------

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Strona1"">1</a>"
                            + @"<a class=""btn btn default btn-primary selected"" href=""Strona2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Strona3"">3</a>", result.ToString());

        }

        [TestMethod]
        public void Can_Paginate()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"},
                }
                );

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            var result = (ProductListViewModel)controller.List(null, 2).Model;


            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");


        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"},
                }
                );

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            var result = (ProductListViewModel)controller.List(null, 2).Model;


            var pageinfo = result.PagingInfo;
            Assert.AreEqual(pageinfo.CurrentPage, 2);
            Assert.AreEqual(pageinfo.ItemsPerPage, 3);
            Assert.AreEqual(pageinfo.TotalItems, 5);
            Assert.AreEqual(pageinfo.TotalPages, 2);

        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1",},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"},
                }
                );

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            var result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();


            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");

        }

        [TestMethod]
        public void CanGenerateCategories()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1",},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"},
                }
                );

            NavController target = new NavController(mock.Object);

            var results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Cat1");
            Assert.AreEqual(results[1], "Cat2");
            Assert.AreEqual(results[2], "Cat3");

        }

        [TestMethod]
        public void IndicatesSelectedCategory()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1",},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"},
                }
                );

            NavController target = new NavController(mock.Object);
            string categoryToSelect = "Cat2";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);

        }

        [TestMethod]
        public void GenerateCategorySpecificProductCount()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Category="Cat1",},
                    new Product {ProductID = 2, Name = "P2", Category="Cat2"},
                    new Product {ProductID = 3, Name = "P3", Category="Cat1"},
                    new Product {ProductID = 4, Name = "P4", Category="Cat2"},
                    new Product {ProductID = 5, Name = "P5", Category="Cat3"},
                }
                );

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            int res1 = ((ProductListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);

        }
    }
}
