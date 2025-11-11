using System.ComponentModel.DataAnnotations;

namespace _24DH113309_MyStore.Models
{
    public class OrderDetailMetadata
    {
        [Display(Name = "Mã chi tiết đơn hàng")]
        public int OrderDetailID { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public int OrderID { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int ProductID { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, 9999, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
        [Range(0, 1000000000, ErrorMessage = "Đơn giá không hợp lệ")]
        public decimal UnitPrice { get; set; }
    }
}
