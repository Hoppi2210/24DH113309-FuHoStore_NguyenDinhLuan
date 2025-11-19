using _24DH113309_MyStore.Models;
using PagedList;
using System.Collections.Generic;
using System.Linq;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class CartIndexVM
    {
        // Yêu cầu 1: Items được nhóm theo Tên Danh mục
        public List<IGrouping<string, CartItem>> GroupedItems { get; set; }

        // Yêu cầu 2: Sản phẩm tương tự (có phân trang)
        public IPagedList<Product> SimilarProducts { get; set; }

        // Dữ liệu bổ sung: Tổng tiền
        public decimal TotalValue { get; set; }

        public CartIndexVM()
        {
            GroupedItems = new List<IGrouping<string, CartItem>>();
        }
    }
}