using System.Linq;
using System.Web.Mvc;
using PagedList;
using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels; // Thêm
using System.Data.Entity; // Thêm

namespace _24DH113309_MyStore.Controllers
{
    public class HomeController : Controller
    {
        MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(int? page, int? categoryId, string search)
        {
            // 🔹 Load danh mục
            ViewBag.Categories = db.Categories.ToList();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;
            // 🔹 Query sản phẩm
            var products = db.Products.AsQueryable();
            if (categoryId != null)
                products = products.Where(x => x.CategoryID == categoryId);
            if (!string.IsNullOrEmpty(search))
                products = products.Where(x => x.ProductName.Contains(search));
            // 🔹 Phân trang
            int pageSize = 12;
            int pageNumber = page ?? 1;
            return View(products.OrderBy(x => x.ProductID)
                                .ToPagedList(pageNumber, pageSize));
        }

        // ==========================================================
        // 🌟 SỬA: ACTION CHI TIẾT SẢN PHẨM (PRODUCTDETAIL)
        // ==========================================================
        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Product pro = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            // Khởi tạo ViewModel
            var model = new ProductDetailVM();

            // 1. Gán sản phẩm chính
            model.product = pro;
            if (quantity.HasValue)
            {
                model.quantity = quantity.Value;
            }

            // 2. Lấy tất cả sản phẩm cùng danh mục (trừ sản phẩm hiện tại)
            var relatedQuery = db.Products
                .Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID);

            // 3. Gán danh sách sản phẩm tương tự (Lấy 8 sản phẩm, không phân trang)
            model.RelatedProducts = relatedQuery
                .OrderBy(p => p.ProductID)
                .Take(8)
                .ToList();

            // 4. Gán danh sách sản phẩm bán chạy (có phân trang)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Lấy từ VM

            model.TopProducts = relatedQuery
                .OrderByDescending(p => p.OrderDetails.Count()) // Sắp xếp theo số lượng đã bán
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }
    }
}