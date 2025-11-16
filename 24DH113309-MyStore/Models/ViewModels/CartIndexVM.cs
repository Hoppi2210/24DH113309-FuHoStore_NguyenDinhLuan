using _24DH113309_MyStore.Models;
using System.Collections.Generic;
using System.Linq;
using PagedList;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class CartIndexVM
    {
        // Yêu cầu 1: Items grouped by Category Name
        public List<IGrouping<string, CartItem>> GroupedItems { get; set; }

        // Yêu cầu 2: Similar products, paginated
        public IPagedList<Product> SimilarProducts { get; set; }

        // Dữ liệu bổ sung
        public decimal TotalValue { get; set; }

        public CartIndexVM()
        {
            GroupedItems = new List<IGrouping<string, CartItem>>();
        }
    }
}