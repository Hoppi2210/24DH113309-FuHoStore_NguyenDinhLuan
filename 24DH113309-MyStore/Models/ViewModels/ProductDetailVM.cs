using _24DH113309_MyStore.Models;
using PagedList;
using System.Collections.Generic;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class ProductDetailVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;

        // Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;

        // Danh sách 8 sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // Danh sách 8 sản phẩm bán chạy nhất cùng danh mục (có phân trang)
        public IPagedList<Product> TopProducts { get; set; }
    }
}