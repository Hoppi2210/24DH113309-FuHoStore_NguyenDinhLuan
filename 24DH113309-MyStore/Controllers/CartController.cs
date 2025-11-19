using _24DH113309_MyStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using _24DH113309_MyStore.Models.ViewModels;
using PagedList;
using System.Data.Entity;
using System.Collections.Generic;

namespace _24DH113309_MyStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        private int GetCartId(int customerId)
        {
            var cart = db.Carts.FirstOrDefault(x => x.CustomerID == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart.CartID;
        }

        
        // ACTION GIỎ HÀNG NÂNG CAO (INDEX)
       
        public ActionResult Index(int? page)
        {
            if (Session["CUS"] == null)
                return RedirectToAction("Login", "Account");
            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);

            // Lấy các item trong giỏ, include Product và Category
            var items = db.CartItems
                .Include(ci => ci.Product.Category)
                .Where(x => x.CartID == cartId)
                .ToList();

            var model = new CartIndexVM();

            // 1. Nhóm các sản phẩm theo Tên Danh mục
            model.GroupedItems = items.GroupBy(item => item.Product.Category.CategoryName).ToList();

            // 2. Tính tổng tiền
            model.TotalValue = items.Sum(x => x.UnitPrice * x.Quantity);

            // 3. Lấy sản phẩm tương tự
            var cartCategoryNames = items.Select(ci => ci.Product.Category.CategoryName).Distinct().ToList();
            var cartProductIDs = items.Select(ci => ci.ProductID).ToList();

            var similarProductsQuery = db.Products
                .Include(p => p.Category)
                .Where(p => cartCategoryNames.Contains(p.Category.CategoryName) && !cartProductIDs.Contains(p.ProductID))
                .OrderBy(p => p.ProductID);

            // 4. Phân trang sản phẩm tương tự
            int pageNumber = page ?? 1;
            int pageSize = 6;
            model.SimilarProducts = similarProductsQuery.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // ================= THÊM GIỎ =================
        // Sửa: Thêm tham số "int quantity"
        public ActionResult Add(int productId, int quantity = 1)
        {
            if (Session["CUS"] == null)
                return RedirectToAction("Login", "Account");

            // Đảm bảo số lượng luôn ít nhất là 1
            if (quantity < 1)
            {
                quantity = 1;
            }

            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);
            var product = db.Products.Find(productId);
            if (product == null)
                return HttpNotFound();

            var item = db.CartItems
                .FirstOrDefault(x => x.CartID == cartId && x.ProductID == productId);

            if (item == null)
            {
                db.CartItems.Add(new CartItem
                {
                    CartID = cartId,
                    ProductID = productId,
                    Quantity = quantity, 
                    UnitPrice = product.ProductPrice
                });
            }
            else
            {
                item.Quantity += quantity; 
            }
            db.SaveChanges();
            return RedirectToAction("Index"); // Chuyển về trang giỏ hàng
        }

        // ========== CẬP NHẬT SỐ LƯỢNG ==========
        [HttpPost]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);
            var item = db.CartItems.FirstOrDefault(x => x.CartID == cartId && x.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ========== XÓA 1 SẢN PHẨM ==========
        [HttpPost]
        public ActionResult RemoveItem(int id)
        {
            var item = db.CartItems.Find(id);
            if (item != null)
            {
                db.CartItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ========== XÓA TẤT CẢ ==========
        public ActionResult Clear()
        {
            if (Session["CUS"] == null)
                return RedirectToAction("Login", "Account");
            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);
            var items = db.CartItems.Where(x => x.CartID == cartId);
            db.CartItems.RemoveRange(items);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ==========================================================
        // CHECKOUT (GET) - BỎ [Authorize]
        // ==========================================================
        public ActionResult Checkout()
        {
            if (Session["CUS"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán!";
                return RedirectToAction("Login", "Account");
            }

            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);
            var items = db.CartItems
                .Include(ci => ci.Product)
                .Where(x => x.CartID == cartId)
                .ToList();

            if (!items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index");
            }

            // Tạo ViewModel
            var model = new CheckoutVM
            {
                CartItems = items,
                TotalAmount = items.Sum(x => x.UnitPrice * x.Quantity),
                CustomerID = cus.CustomerID,
                Username = cus.Username,
                FullName = cus.CustomerName,
                Phone = cus.CustomerPhone,
                DetailAddress = cus.CustomerAddress,
                PaymentMethod = "COD"
            };

            return View(model);
        }

        // ==========================================================
        // CHECKOUT (POST) - BỎ [Authorize]
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (Session["CUS"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán!";
                return RedirectToAction("Login", "Account");
            }

            var cus = (Customer)Session["CUS"];
            int cartId = GetCartId(cus.CustomerID);

            if (!ModelState.IsValid)
            {
                model.CartItems = db.CartItems
                    .Include(ci => ci.Product)
                    .Where(x => x.CartID == cartId)
                    .ToList();
                model.TotalAmount = model.CartItems.Sum(x => x.UnitPrice * x.Quantity);
                return View(model);
            }

            // Lấy lại thông tin giỏ hàng từ DB
            var cartItemsFromDb = db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartID == cartId)
                .ToList();

            if (!cartItemsFromDb.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index");
            }

            model.CartItems = cartItemsFromDb;
            model.TotalAmount = cartItemsFromDb.Sum(x => x.UnitPrice * x.Quantity);
            model.CustomerID = cus.CustomerID;

            // KIỂM TRA PHƯƠNG THỨC THANH TOÁN
            if (model.PaymentMethod == "Paypal")
            {
                return RedirectToAction("PaymentWithPaypal", "Paypal", model);
            }

            // XỬ LÝ ĐƠN HÀNG THƯỜNG (COD, Banking, Momo...)
            try
            {
                var addressDisplay = $"{model.DetailAddress}, {model.Ward}, {model.District}, {model.Province} | {model.FullName} - {model.Phone}";

                var order = new Order
                {
                    CustomerID = cus.CustomerID,
                    OrderDate = DateTime.Now,
                    AddressDelivery = addressDisplay,
                    ShippingAddress = addressDisplay,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = model.PaymentMethod == "COD" ? "Chưa thanh toán" : "Chờ xác nhận",
                    TotalAmount = model.TotalAmount
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Thêm chi tiết đơn hàng
                foreach (var item in model.CartItems)
                {
                    db.OrderDetails.Add(new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                // Xóa giỏ hàng
                db.CartItems.RemoveRange(cartItemsFromDb);
                db.SaveChanges();

                TempData["Success"] = "Đặt hàng thành công!";
                return RedirectToAction("Success", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                model.CartItems = db.CartItems
                    .Include(ci => ci.Product)
                    .Where(x => x.CartID == cartId)
                    .ToList();
                model.TotalAmount = model.CartItems.Sum(x => x.UnitPrice * x.Quantity);
                return View(model);
            }
        }

        // ========== TRANG THÀNH CÔNG - BỎ [Authorize] ==========
        public ActionResult Success(int id)
        {
            if (Session["CUS"] == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account");
            }

            var cus = (Customer)Session["CUS"];
            var order = db.Orders
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null || order.CustomerID != cus.CustomerID)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}