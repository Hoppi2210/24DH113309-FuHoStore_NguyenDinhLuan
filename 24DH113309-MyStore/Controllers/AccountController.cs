using _24DH113309_MyStore.Models;
using System.Web.Mvc;
using System.Linq;

public class AccountController : Controller
{
    private MyStoreEntities db = new MyStoreEntities();

    [HttpGet]
    public ActionResult Login() => View();

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            Session["USER"] = user;
            return RedirectToAction("Index", "Home");
        }
        ViewBag.Error = "Tên đăng nhập hoặc mật khẩu sai!";
        return View();
    }

    public ActionResult Logout()
    {
        Session["USER"] = null;
        return RedirectToAction("Index", "Home");
    }
}
