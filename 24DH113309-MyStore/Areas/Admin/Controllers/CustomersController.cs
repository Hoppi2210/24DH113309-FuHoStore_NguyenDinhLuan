using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using _24DH113309_MyStore.Models;

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            var customers = db.Customers.ToList();
            return View(customers);
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Include User để lấy thông tin Username và Role
            Customer customer = db.Customers.Include(c => c.User).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Admin/Customers/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create (Code đã cung cấp)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer customer, string Password, string ConfirmPassword, HttpPostedFileBase uploadImage)
        {
            // ... (Giữ nguyên logic của bạn ở đây)
            // ✅ Kiểm tra xác nhận mật khẩu
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "❌ Mật khẩu xác nhận không khớp. Vui lòng nhập lại.");
                return View(customer);
            }

            // ✅ Kiểm tra Username có trùng hay không
            var existingUser = db.Users.FirstOrDefault(u => u.Username == customer.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "❌ Tên đăng nhập đã tồn tại, vui lòng chọn tên khác!");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                // ✅ Tạo tài khoản User (Cần lấy vai trò từ form nếu Admin có thể chọn vai trò)
                string userRole = Request.Form["Role"] ?? "Customer"; // Lấy vai trò từ form View

                var newUser = new User
                {
                    Username = customer.Username,
                    Password = Password, // Lưu ý: Cần mã hóa mật khẩu trước khi lưu!
                    UserRole = userRole
                };
                db.Users.Add(newUser);
                // db.SaveChanges(); // Lưu ý: Nên chỉ SaveChanges 1 lần ở cuối

                // ✅ Xử lý ảnh đại diện
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Content/Images/Customers");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Path.GetFileName(uploadImage.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    uploadImage.SaveAs(filePath);
                    customer.CustomerImage = fileName;
                }
                else
                {
                    customer.CustomerImage = "default.jpg";
                }

                // ✅ Gán ngày tạo & lưu khách hàng
                customer.DateCreated = DateTime.Now;
                db.Customers.Add(customer);
                db.SaveChanges(); // Lưu tất cả thay đổi (User và Customer)

                TempData["SuccessMessage"] = "✅ Thêm khách hàng thành công!";
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Customer customer = db.Customers.Include(c => c.User).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null) return HttpNotFound();

            // Gửi mật khẩu hiện tại sang View (DÙNG CHO TRƯỜNG ẨN HOẶC HIỂN THỊ)
            ViewBag.CurrentPassword = customer.User?.Password;
            ViewBag.UserRole = new SelectList(new List<string> { "Customer", "Staff" }, customer.User?.UserRole);

            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer, string NewPassword, string ConfirmPassword, HttpPostedFileBase uploadImage)
        {
            // Lấy các trường ẩn hoặc không gửi qua form để tránh mất dữ liệu (NoTracking)
            var existingCustomer = db.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerID == customer.CustomerID);
            var userToUpdate = db.Users.FirstOrDefault(u => u.Username == existingCustomer.Username);

            if (userToUpdate == null) return HttpNotFound();

            // Kiểm tra mật khẩu mới
            if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "❌ Mật khẩu mới và xác nhận không khớp.");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                // 1. Xử lý ảnh đại diện
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Content/Images/Customers");
                    string fileName = Path.GetFileName(uploadImage.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    uploadImage.SaveAs(filePath);
                    customer.CustomerImage = fileName;
                }
                else
                {
                    // Giữ lại ảnh cũ
                    customer.CustomerImage = existingCustomer.CustomerImage;
                }

                // 2. Cập nhật User
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    userToUpdate.Password = NewPassword; // Cần mã hóa
                }
                userToUpdate.UserRole = Request.Form["UserRole"]; // Cập nhật role từ form Edit
                db.Entry(userToUpdate).State = EntityState.Modified;

                // 3. Cập nhật Customer
                customer.Username = existingCustomer.Username; // Giữ nguyên Username liên kết
                customer.DateCreated = existingCustomer.DateCreated; // Giữ nguyên ngày tạo
                db.Entry(customer).State = EntityState.Modified;

                db.SaveChanges();
                TempData["SuccessMessage"] = "✅ Cập nhật khách hàng thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.UserRole = new SelectList(new List<string> { "Customer", "Staff" }, userToUpdate.UserRole);
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = db.Customers.Include(c => c.User).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            var userToDelete = db.Users.FirstOrDefault(u => u.Username == customer.Username);

            
            // 1. Xóa Customer
            db.Customers.Remove(customer);

            // 2. Xóa User liên quan
            if (userToDelete != null)
            {
                db.Users.Remove(userToDelete);
            }

            db.SaveChanges();
            TempData["SuccessMessage"] = "❌ Đã xóa khách hàng thành công!";
            return RedirectToAction("Index");
        }

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