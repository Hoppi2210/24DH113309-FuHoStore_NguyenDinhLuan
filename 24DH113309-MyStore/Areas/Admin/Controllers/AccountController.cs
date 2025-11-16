using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // ===== GET: Admin/Account/Login =====
        public ActionResult Login()
        {
            return View();
        }

        // ===== POST: Admin/Account/Login =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // 1️⃣ Chỉ kiểm tra Username và Password trước
                var user = db.Users.SingleOrDefault(u =>
                    u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == model.Password
                );

                // 2️⃣ Kiểm tra xem user có tồn tại VÀ có quyền Admin hoặc Staff không
                if (user != null &&
                   (user.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    user.UserRole.Equals("Staff", StringComparison.OrdinalIgnoreCase)))
                {
                    // 3️⃣ Tạo vé xác thực (FormsAuthenticationTicket)
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        user.Username,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(60),
                        false,
                        user.UserRole.Trim() // Lưu role THỰC TẾ (Admin hoặc Staff) vào vé
                    );

                    // 4️⃣ Mã hóa vé và tạo cookie
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true
                    };
                    Response.Cookies.Add(authCookie);

                    // 5️⃣ Xóa session cũ (nếu có)
                    Session.Clear();

                    // 6️⃣ Chuyển đến trang chủ Admin
                    return RedirectToAction("Index", "Home");
                }

                // ❌ Sai tài khoản hoặc không có quyền Admin/Staff
                ModelState.AddModelError("", "Sai tên đăng nhập, mật khẩu hoặc bạn không có quyền truy cập.");
            }

            return View(model);
        }

        // ===== GET: Admin/Account/Logout =====
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}