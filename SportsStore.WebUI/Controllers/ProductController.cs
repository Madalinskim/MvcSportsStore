using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        public ViewResult List(string category, int page = 1)
        {
            ProductListViewModel model = new ProductListViewModel
            {
                Products = repository.Products.Where(s => category == null || category == s.Category)
                                              .OrderBy(s => s.ProductID)
                                              .Skip((page - 1) * PageSize)
                                              .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                                 repository.Products.Count() :
                                 repository.Products.Where(s => s.Category == category).Count()
                },

                CurrentCategory = category
            };

            return View(model);

        }
    }
}