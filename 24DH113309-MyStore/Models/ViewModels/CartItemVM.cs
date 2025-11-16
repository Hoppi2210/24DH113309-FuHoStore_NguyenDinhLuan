using System;

namespace _24DH113309_MyStore.Models.ViewModels
{
    [Serializable]
    public class CartItemVM
    {
        public int ProductID { get; set; }
        public string Name { get; set; }      // tên sản phẩm
        public decimal Price { get; set; }    // giá bán
        public int Qty { get; set; }          // số lượng
        public string Image { get; set; }     // ảnh

        public decimal Total => Price * Qty;  // tổng cộng
    }
}
