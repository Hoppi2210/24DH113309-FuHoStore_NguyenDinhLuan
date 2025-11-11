using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;

namespace _24DH113309_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        // 🏠 Trang chủ: hiển thị danh mục và sản phẩm mới
        public ActionResult Index()
        {
            ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
            var products = db.Products.OrderByDescending(p => p.ProductID).Take(12).ToList();
            return View(products);
        }

        // 📂 Danh sách theo danh mục
        public ActionResult Category(int id)
        {
            var cate = db.Categories.Find(id);
            if (cate == null) return HttpNotFound();

            ViewBag.Category = cate;
            var items = db.Products.Where(p => p.CategoryID == id)
                                   .OrderByDescending(p => p.ProductID).ToList();
            return View(items);
        }

        // 🔍 Chi tiết sản phẩm
        public ActionResult Detail(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();

            // Sản phẩm liên quan cùng danh mục
            ViewBag.Related = db.Products
                                .Where(x => x.CategoryID == p.CategoryID && x.ProductID != id)
                                .OrderByDescending(x => x.ProductID)
                                .Take(6)
                                .ToList();

            return View(p);
        }

        // 🔎 Tìm kiếm sản phẩm
        public ActionResult Search(string q)
        {
            ViewBag.Q = q;
            var rs = string.IsNullOrWhiteSpace(q)
                ? db.Products.Take(0).ToList()
                : db.Products.Where(p => p.ProductName.Contains(q) ||
                                         p.ProductDecription.Contains(q))
                             .ToList();
            return View(rs);
        }
    }
}
