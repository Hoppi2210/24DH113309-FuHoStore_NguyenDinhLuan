using System;
using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models
{
    public class OrderMetadata
    {
        [Display(Name = "Mã đơn hàng")]
        public int OrderID { get; set; }

        [Display(Name = "Mã khách hàng")]
        public int CustomerID { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không hợp lệ")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        [StringLength(50)]
        public string PaymentStatus { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [StringLength(200)]
        public string AddressDelivery { get; set; }
    }
}
