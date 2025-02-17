
using KKShop.DomainModels;

namespace KKShop.Shop.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}
