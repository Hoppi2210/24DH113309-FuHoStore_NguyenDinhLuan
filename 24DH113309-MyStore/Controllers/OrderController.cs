using _24DH113309_MyStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Collections.Generic;
using PagedList;

namespace _24DH113309_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        // Lấy CartID của customer
        private int GetCartId(int customerId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.CustomerID == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart.CartID;
        }

        // ==========================================================
        // ACTION LỊCH SỬ MUA HÀNG (MYORDER)
        // ==========================================================
        public ActionResult MyOrder()
        {
            // 1. Lấy Customer từ Session
            if (!(Session["CUS"] is Customer cus))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Lấy danh sách đơn hàng của khách hàng đó
            var orders = db.Orders
                .Where(o => o.CustomerID == cus.CustomerID)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            // 3. Trả về View
            return View(orders);
        }

        // Nhập địa chỉ bằng text
        public ActionResult Address()
        {
            return View();
        }

        // Dispose để giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}