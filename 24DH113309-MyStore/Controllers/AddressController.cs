using System.Web.Mvc;

namespace _24DH113309_MyStore.Controllers
{
    public class AddressController : Controller
    {
        // GET: Nhập địa chỉ
        public ActionResult Index()
        {
            return View();
        }

        // POST: Lưu địa chỉ
        [HttpPost]
        public ActionResult Save(string fullName, string phone, string addressDetail, string type)
        {
            TempData["ShippingAddress"] =
                $"{fullName} - {phone}\n{addressDetail}\nLoại địa chỉ: {type}";

            return RedirectToAction("Checkout", "Order");
        }
    }
}
