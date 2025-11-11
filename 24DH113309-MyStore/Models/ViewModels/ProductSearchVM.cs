using PagedList;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }       
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public IPagedList<Product> Results { get; set; }
    }
}
