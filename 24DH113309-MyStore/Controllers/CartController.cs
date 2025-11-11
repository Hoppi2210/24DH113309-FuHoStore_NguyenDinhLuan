using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels;

namespace _24DH113309_MyStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();
        private const string CART_KEY = "CART";

        // Lấy giỏ hàng từ Session
        private List<CartItemVM> Cart
        {
            get
            {
                if (Session[CART_KEY] == null)
                    Session[CART_KEY] = new List<CartItemVM>();
                return (List<CartItemVM>)Session[CART_KEY];
            }
            set { Session[CART_KEY] = value; }
        }

        // 🛒 Trang hiển thị giỏ hàng
        public ActionResult Index()
        {
            return View(Cart);
        }

        // 🟢 Thêm sản phẩm vào giỏ hàng
        public ActionResult AddItem(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            var existing = Cart.FirstOrDefault(p => p.ProductID == id);
            if (existing != null)
            {
                existing.Qty++;
            }
            else
            {
                Cart.Add(new CartItemVM
                {
                    ProductID = product.ProductID,
                    Name = product.ProductName,
                    Price = product.ProductPrice,
                    Image = product.ProductImage,  // ✅ khớp với CartItemVM.Image
                    Qty = 1
                });
            }

            // Cập nhật lại Session
            Session[CART_KEY] = Cart;

            // ✅ Quay lại trang trước kèm thông báo Toast
            var referrer = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : Url.Action("Index", "Home");
            return Redirect(referrer + "?added=true");
        }

        // 🔴 Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveItem(int id)
        {
            var item = Cart.FirstOrDefault(p => p.ProductID == id);
            if (item != null)
            {
                Cart.Remove(item);
                Session[CART_KEY] = Cart;
            }
            return RedirectToAction("Index");
        }

        // 🧹 Xóa toàn bộ giỏ
        public ActionResult Clear()
        {
            Cart.Clear();
            Session[CART_KEY] = Cart;
            return RedirectToAction("Index");
        }
    }
}
