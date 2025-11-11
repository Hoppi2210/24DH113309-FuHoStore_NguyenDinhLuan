using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _24DH113309_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var vm = new ProductSearchVM();
            vm.SearchTerm = searchTerm;
            vm.MinPrice = minPrice;
            vm.MaxPrice = maxPrice;

            var products = db.Products.AsQueryable();

            // --- Lọc ---
            if (!string.IsNullOrEmpty(searchTerm))
                products = products.Where(p => p.ProductName.Contains(searchTerm));

            if (minPrice.HasValue)
                products = products.Where(p => p.ProductPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);

            // --- Sắp xếp ---
            switch (sortOrder)
            {
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                default: products = products.OrderBy(p => p.ProductName); break;
            }

            // --- Phân trang ---
            int pageSize = 6;                    // số sản phẩm / trang
            int pageNumber = (page ?? 1);        // trang mặc định
            vm.Results = products.ToPagedList(pageNumber, pageSize);  

            ViewBag.SortOrder = sortOrder;

            return View(vm);
        }



        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Include(p => p.Category)
                                     .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            return View(product);  // ✅ Trả về 1 Product duy nhất
        }



        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload hình
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    product.ProductImage = "/Images/" + fileName;
                }

                db.Products.Add(product);
                db.SaveChanges();
                TempData["Message"] = "✅ Đã thêm sản phẩm mới!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload lại hình nếu có file mới
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    product.ProductImage = "/Images/" + fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "💾 Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var product = db.Products.Include(p => p.Category)
                                     .FirstOrDefault(p => p.ProductID == id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["Message"] = "🗑 Đã xóa sản phẩm.";
            return RedirectToAction("Index");
        }
    }
}
