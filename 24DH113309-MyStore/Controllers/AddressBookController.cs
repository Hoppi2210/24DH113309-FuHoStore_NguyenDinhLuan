using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;

namespace _24DH113309_MyStore.Controllers
{
    public class AddressBookController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        private Customer GetCurrentUser()
        {
            return Session["CUS"] as Customer;
        }

        // ======================= INDEX =========================
        public ActionResult Index()
        {
            var cus = GetCurrentUser();
            if (cus == null) return RedirectToAction("Login", "Account");

            var list = db.ShippingAddresses
                .Where(x => x.CustomerID == cus.CustomerID)
                .OrderByDescending(x => x.IsDefault)
                .ToList();

            return View(list);
        }


        // ======================= CREATE (GET) =========================
        public ActionResult Create()
        {
            if (GetCurrentUser() == null)
                return RedirectToAction("Login", "Account");

            return View(new ShippingAddress());
        }


        // ======================= CREATE (POST) =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ShippingAddress model)
        {
            var cus = GetCurrentUser();
            if (cus == null) return RedirectToAction("Login", "Account");

            model.CustomerID = cus.CustomerID;

            // Nếu chọn mặc định
            if (model.IsDefaultBool)
            {
                var all = db.ShippingAddresses.Where(x => x.CustomerID == cus.CustomerID);
                foreach (var a in all) a.IsDefault = false;
            }

            db.ShippingAddresses.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // ======================= EDIT (GET) =========================
        public ActionResult Edit(int id)
        {
            var cus = GetCurrentUser();
            if (cus == null) return RedirectToAction("Login", "Account");

            var addr = db.ShippingAddresses.Find(id);
            if (addr == null) return HttpNotFound();

            return View(addr);
        }


        // ======================= EDIT (POST) =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ShippingAddress model)
        {
            var cus = GetCurrentUser();
            if (cus == null) return RedirectToAction("Login", "Account");

            var addr = db.ShippingAddresses.Find(model.AddressID);
            if (addr == null) return HttpNotFound();

            addr.FullName = model.FullName;
            addr.Phone = model.Phone;
            addr.Province = model.Province;
            addr.District = model.District;
            addr.Ward = model.Ward;
            

            if (model.IsDefaultBool)
            {
                var all = db.ShippingAddresses.Where(x => x.CustomerID == cus.CustomerID);
                foreach (var a in all) a.IsDefault = false;
            }

            addr.IsDefault = model.IsDefaultBool;

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // ======================= SET DEFAULT =========================
        [HttpPost]
        public ActionResult SetDefault(int id)
        {
            var cus = GetCurrentUser();
            if (cus == null) return RedirectToAction("Login", "Account");

            var all = db.ShippingAddresses.Where(x => x.CustomerID == cus.CustomerID);
            foreach (var a in all) a.IsDefault = false;

            var target = db.ShippingAddresses.Find(id);
            if (target != null)
                target.IsDefault = true;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
