namespace KKShop.Shop.Models
{
    public class PaginationSearchInput
    {
        /// <summary>
        /// trang can hien thi
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// So dong hien thi tren moi trang
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Chuoi gia tri can tim kiem
        /// </summary>
        public string SearchValue { get; set; } = "";
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }
}
