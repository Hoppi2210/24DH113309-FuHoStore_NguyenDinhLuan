using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels;

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Home
        public ActionResult Index()
        {
            // ---- A. Thống kê khách hàng theo tháng tạo ----
            var chartDataCustomer = db.Customers
                .Where(c => c.DateCreated.HasValue)
                .GroupBy(c => c.DateCreated.Value.Month)
                .Select(g => new ChartVM
                {
                    Category = "Tháng " + g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Category)
                .ToList();

            // ---- B. Thống kê sản phẩm theo loại hàng ----
            var chartDataProduct = db.Products
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new ChartVM
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Category)
                .ToList();

            // ---- C. Gửi dữ liệu ra view ----
            ViewBag.CustomerChart = chartDataCustomer;
            ViewBag.ProductChart = chartDataProduct;

            return View();

        }
    }
}
