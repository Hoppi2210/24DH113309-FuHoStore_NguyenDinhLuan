using _24DH113309_MyStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class CheckoutVM
    {
        // Thông tin giỏ hàng
        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }

        // Thông tin khách hàng (ẩn)
        public int CustomerID { get; set; }
        public string Username { get; set; }

        // Thông tin nhập trên Form
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành")]
        [Display(Name = "Tỉnh/Thành")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập quận/huyện")]
        [Display(Name = "Quận/Huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phường/xã")]
        [Display(Name = "Phường/Xã")]
        public string Ward { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết")]
        [Display(Name = "Địa chỉ chi tiết")]
        public string DetailAddress { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } // "COD", "Banking", "Momo", "Paypal"
    }
}