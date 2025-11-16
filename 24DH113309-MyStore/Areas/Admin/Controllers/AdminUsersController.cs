using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;
using System.Data.Entity.Validation; 

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    
    public class AdminUsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private List<string> GetRoles() => new List<string> { "Admin", "Staff" };

        // GET: Admin/AdminUsers
        public ActionResult Index()
        {
            var adminUsers = db.Users.Where(u => u.UserRole == "Admin" || u.UserRole == "Staff").ToList();
            return View(adminUsers);
        }

        // GET: Admin/AdminUsers/Create
        public ActionResult Create()
        {
            ViewBag.UserRole = new SelectList(GetRoles());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                }
                else
                {
                    try
                    {
                        // Cần mã hóa mật khẩu ở đây
                        db.Users.Add(user);
                        db.SaveChanges(); // Lỗi xảy ra ở đây
                        TempData["SuccessMessage"] = "Thêm tài khoản quản trị thành công!";
                        return RedirectToAction("Index");
                    }
                    
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                
                                ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                }
            }

            // Nếu có lỗi (hoặc ModelState không hợp lệ), quay lại form
            ViewBag.UserRole = new SelectList(GetRoles(), user.UserRole);
            return View(user);
        }

        // GET: Admin/AdminUsers/Edit/username
        public ActionResult Edit(string username)
        {
            if (string.IsNullOrEmpty(username)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User user = db.Users.Find(username);
            if (user == null) return HttpNotFound();
            ViewBag.UserRole = new SelectList(GetRoles(), user.UserRole);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            var existingUser = db.Users.AsNoTracking().FirstOrDefault(u => u.Username == user.Username);

            // Xóa lỗi validation của Password nếu không nhập
            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.Remove("Password");
            }

            if (ModelState.IsValid)
            {
                // Giữ mật khẩu cũ nếu không nhập mật khẩu mới
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = existingUser?.Password;
                }

                try
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }
            ViewBag.UserRole = new SelectList(GetRoles(), user.UserRole);
            return View(user);
        }

        // GET: Admin/AdminUsers/Delete/username
        public ActionResult Delete(string username)
        {
            if (string.IsNullOrEmpty(username)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User user = db.Users.Find(username);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        // POST: Admin/AdminUsers/Delete/username
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string username)
        {
            User user = db.Users.Find(username);
            // Bảo vệ: Không cho phép tự xóa tài khoản đang đăng nhập
            if (user.Username == User.Identity.Name)
            {
                TempData["ErrorMessage"] = "Không thể tự xóa tài khoản đang đăng nhập.";
                return RedirectToAction("Index");
            }
            db.Users.Remove(user);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã xóa tài khoản.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { db.Dispose(); }
            base.Dispose(disposing);
        }
    }
}