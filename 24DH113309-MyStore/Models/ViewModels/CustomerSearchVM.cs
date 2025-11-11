using PagedList;
using _24DH113309_MyStore.Models;

namespace _24DH113309_MyStore.Models.ViewModels
{
    public class CustomerSearchVM
    {
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public IPagedList<Customer> Results { get; set; }
    }
}