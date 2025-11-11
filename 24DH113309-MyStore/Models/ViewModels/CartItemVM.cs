using System;

namespace _24DH113309_MyStore.Models.ViewModels
{
    [Serializable]
    public class CartItemVM
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal Total => UnitPrice * Quantity;
    }
}
