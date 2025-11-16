using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;

namespace _24DH113309_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // ====================== LOGIN ======================
        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username && c.Password == password);

            if (customer != null)
            {
                Session["CUS"] = customer;

                TempData["Success"] = "Đăng nhập thành công!";

                // 🔥 QUAY VỀ TRANG CHỦ — KHÔNG chuyển sang Trang Thông tin tài khoản
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }


        // ====================== REGISTER ======================
        public ActionResult Register() => View();

        [HttpPost]
        public ActionResult Register(Customer model, string passwordConfirm)
        {
            if (model.Password != passwordConfirm)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp!";
                return View(model);
            }

            if (db.Customers.Any(c => c.Username == model.Username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View(model);
            }

            db.Customers.Add(model);
            db.SaveChanges();
            Session["CUS"] = model;

            return RedirectToAction("AccountInfo");
        }

        // ====================== LOGOUT ======================
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ====================== THÔNG TIN TÀI KHOẢN ======================
        public ActionResult AccountInfo()
        {
            if (Session["CUS"] == null)
                return RedirectToAction("Login");

            var cus = (Customer)Session["CUS"];
            return View(cus);
        }


        // ====================== CHỈNH SỬA THÔNG TIN ======================
        public ActionResult EditInfo()
        {
            if (Session["CUS"] == null)
                return RedirectToAction("Login");

            var cus = (Customer)Session["CUS"];
            var data = db.Customers.Find(cus.CustomerID);

            return View(data);
        }

        [HttpPost]
        public ActionResult EditInfo(Customer model)
        {
            var cus = db.Customers.Find(model.CustomerID);
            cus.CustomerName = model.CustomerName;
            cus.CustomerPhone = model.CustomerPhone;
            cus.CustomerAddress = model.CustomerAddress;

            db.SaveChanges();

            Session["CUS"] = cus; // Update session

            ViewBag.Success = "Cập nhật thành công!";
            return View(cus);
        }
        // =========================
        // TRANG THÔNG TIN TÀI KHOẢN
        // =========================
        public ActionResult ProfileInfo()
        {
            var cus = Session["CUS"] as Customer;
            if (cus == null) return RedirectToAction("Login");

            return View(cus);
        }

        // =========================
        // CẬP NHẬT THÔNG TIN
        // =========================
        [HttpPost]
        public ActionResult UpdateProfile(Customer model)
        {
            var cus = db.Customers.Find(model.CustomerID);
            if (cus == null) return HttpNotFound();

            cus.CustomerName = model.CustomerName;
            cus.CustomerPhone = model.CustomerPhone;
            cus.CustomerEmail = model.CustomerEmail;

            db.SaveChanges();

            Session["CUS"] = cus; // update session

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ProfileInfo");
        }

        // =========================
        // ĐỔI MẬT KHẨU
        // =========================
        [HttpPost]
        public ActionResult ChangePassword(int id, string oldPass, string newPass)
        {
            var cus = db.Customers.Find(id);

            if (cus.Password != oldPass)
            {
                TempData["Error"] = "Mật khẩu cũ không đúng!";
                return RedirectToAction("ProfileInfo");
            }

            cus.Password = newPass;
            db.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ProfileInfo");
        }

    }
}
