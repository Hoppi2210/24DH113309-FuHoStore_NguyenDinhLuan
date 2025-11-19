using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;
using PagedList;

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Danh sách trạng thái thanh toán/đơn hàng
        private List<string> GetPaymentStatusList()
        {
            return new List<string> {
                "Chờ xử lý", "Đang vận chuyển", "Đã giao",
                "Đã thanh toán", "Chưa thanh toán", "Đã hủy"
            };
        }

        // 1. GET: Admin/Orders
        public ActionResult Index(int? page)
        {
            var orders = db.Orders.Include(o => o.Customer).OrderByDescending(o => o.OrderDate);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        // 2. GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Order order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName");
            return View();
        }

        // POST: Admin/Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,TotalAmount,PaymentStatus,AddressDelivery")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = System.DateTime.Now;
                db.Orders.Add(order);
                db.SaveChanges();
                TempData["SuccessMessage"] = "✅ Thêm đơn hàng thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CustomerName", order.CustomerID);
            return View(order);
        }

        // 3. GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            var result = Details(id);
            if (result is ViewResult viewResult)
            {
                return View(viewResult.Model);
            }
            return result;
        }

        // 4. POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            // 1. Xóa "con" (Chi tiết đơn hàng)
            db.OrderDetails.RemoveRange(db.OrderDetails.Where(od => od.OrderID == id));

            // 2. Xóa "cha" (Đơn hàng)
            db.Orders.Remove(order);

            db.SaveChanges();
            TempData["SuccessMessage"] = "❌ Đã xóa đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        // 5. GET/POST: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Order order = db.Orders.Include(o => o.Customer).FirstOrDefault(o => o.OrderID == id);
            if (order == null) { return HttpNotFound(); }
            ViewBag.PaymentStatusList = new SelectList(GetPaymentStatusList(), order.PaymentStatus);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,PaymentStatus,AddressDelivery")] Order order)
        {
            var existingOrder = db.Orders.AsNoTracking().FirstOrDefault(o => o.OrderID == order.OrderID);
            if (existingOrder == null) { return HttpNotFound(); }

            if (ModelState.IsValid)
            {
                order.OrderDate = existingOrder.OrderDate;
                order.TotalAmount = existingOrder.TotalAmount;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "✅ Cập nhật đơn hàng thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.PaymentStatusList = new SelectList(GetPaymentStatusList(), order.PaymentStatus);
            return View(order);
        }

        // 6. Dispose
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