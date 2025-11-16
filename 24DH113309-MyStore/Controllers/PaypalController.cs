using _24DH113309_MyStore.Models;
using _24DH113309_MyStore.Models.ViewModels;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

// ⭐ SỬA LỖI: Đặt "bí danh" cho các lớp bị trùng tên
using Item = PayPal.Api.Item;
using MyOrder = _24DH113309_MyStore.Models.Order; // <--- Sửa lỗi 'ambiguous reference'

namespace _24DH113309_MyStore.Controllers
{
    [Authorize] // Yêu cầu đăng nhập cho tất cả
    public class PaypalController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        //==================================================
        // CẤU HÌNH PAYPAL
        //==================================================
        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", ConfigurationManager.AppSettings["PayPalMode"] },
                { "connectionTimeout", ConfigurationManager.AppSettings["PayPalConnectionTimeout"] },
                { "clientId", ConfigurationManager.AppSettings["PayPalClientId"] },
                { "clientSecret", ConfigurationManager.AppSettings["PayPalClientSecret"] }
            };
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            return new APIContext(accessToken);
        }

        //==================================================
        // 1. TẠO THANH TOÁN (PAYMENT)
        //==================================================
        public ActionResult PaymentWithPaypal(CheckoutVM model)
        {
            var apiContext = GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    // TẠO MỚI PAYMENT
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/PaymentWithPaypal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, model);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);
                    // Lưu model vào Session để dùng lại sau khi redirect
                    Session.Add("CheckoutModel_" + guid, model);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // THỰC THI THANH TOÁN (SAU KHI REDIRECT VỀ)
                    var guid = Request.Params["guid"];
                    var paymentId = Session[guid] as string;
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                    // Thanh toán thành công -> Lấy lại model và tạo đơn hàng
                    var checkoutModel = Session["CheckoutModel_" + guid] as CheckoutVM;
                    if (checkoutModel == null)
                    {
                        // Lỗi: Mất session, nên quay về trang chủ
                        return RedirectToAction("Index", "Home");
                    }

                    var cus = db.Customers.Find(checkoutModel.CustomerID);
                    if (cus == null)
                    {
                        return View("FailureView"); // Lỗi không tìm thấy khách hàng
                    }

                    var addressDisplay = $"{checkoutModel.DetailAddress}, {checkoutModel.Ward}, {checkoutModel.District}, {checkoutModel.Province} | {checkoutModel.FullName} - {checkoutModel.Phone}";

                    // 2. Tạo Order
                    // ⭐ SỬA LỖI: Dùng "MyOrder" thay vì "Order"
                    MyOrder order = new MyOrder
                    {
                        CustomerID = cus.CustomerID,
                        OrderDate = DateTime.Now,
                        AddressDelivery = addressDisplay,
                        ShippingAddress = addressDisplay,
                        PaymentMethod = "Paypal",
                        PaymentStatus = "Paid", // Đã thanh toán
                        TotalAmount = checkoutModel.TotalAmount
                    };
                    db.Orders.Add(order);
                    db.SaveChanges(); // Lưu để lấy OrderID

                    // 3. Tạo OrderDetails
                    foreach (var item in checkoutModel.CartItems)
                    {
                        db.OrderDetails.Add(new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });
                    }

                    // 4. Xóa giỏ hàng
                    int cartId = GetCartId(cus.CustomerID);
                    var cartItems = db.CartItems.Where(ci => ci.CartID == cartId);
                    db.CartItems.RemoveRange(cartItems);

                    db.SaveChanges(); // Lưu tất cả thay đổi

                    // 5. Xóa Session và chuyển về trang Success
                    Session.Remove(guid);
                    Session.Remove("CheckoutModel_" + guid);
                    return RedirectToAction("Success", "Cart", new { id = order.OrderID });
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu có)
                var a = ex.Message;
                return View("FailureView");
            }
        }

        //==================================================
        // HÀM HELPER: TẠO PAYMENT
        //==================================================
        private Payment CreatePayment(APIContext apiContext, string redirectUrl, CheckoutVM model)
        {
            var itemList = new ItemList() { items = new List<Item>() };
            decimal total = 0;

            foreach (var item in model.CartItems)
            {
                decimal itemPriceUSD = Math.Round(item.UnitPrice / 23000, 2); // Tạm quy đổi USD
                itemList.items.Add(new Item()
                {
                    name = item.Product.ProductName,
                    currency = "USD",
                    price = itemPriceUSD.ToString("F2"), // Định dạng 2 chữ số thập phân
                    quantity = item.Quantity.ToString(),
                    sku = item.ProductID.ToString()
                });
                total += itemPriceUSD * item.Quantity;
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total.ToString("F2") // Định dạng 2 chữ số thập phân
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString("F2"), // Tổng tiền
                details = details
            };

            var transactionList = new List<Transaction>
            {
                new Transaction()
                {
                    description = "Đơn hàng từ FuHo Store",
                    invoice_number = "INV-" + Guid.NewGuid().ToString().Substring(0, 8),
                    amount = amount,
                    item_list = itemList
                }
            };

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }

        //==================================================
        // HÀM HELPER: THỰC THI PAYMENT
        //==================================================
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

        // Lấy CartID của customer
        private int GetCartId(int customerId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.CustomerID == customerId);
            if (cart == null)
            {
                cart = new Cart { CustomerID = customerId };
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart.CartID;
        }

        // View khi thanh toán thất bại
        public ActionResult FailureView()
        {
            return View();
        }
    }
}