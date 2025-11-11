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
                // 1️⃣ Kiểm tra tài khoản (không phân biệt hoa/thường)
                var user = db.Users.SingleOrDefault(u =>
                    u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) &&
                    u.Password == model.Password &&
                    u.UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                );

                if (user != null)
                {
                    // 2️⃣ Tạo vé xác thực (FormsAuthenticationTicket)
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        user.Username,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(60),
                        false,
                        user.UserRole.Trim() // Lưu role vào vé
                    );

                    // 3️⃣ Mã hóa vé và tạo cookie
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true
                    };
                    Response.Cookies.Add(authCookie);

                    // 4️⃣ Xóa session cũ (nếu có)
                    Session.Clear();

                    // 5️⃣ Chuyển đến trang chủ Admin
                    return RedirectToAction("Index", "Home");
                }

                // ❌ Sai tài khoản hoặc không có quyền Admin
                ModelState.AddModelError("", "Sai tên đăng nhập, mật khẩu hoặc bạn không có quyền Admin.");
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
