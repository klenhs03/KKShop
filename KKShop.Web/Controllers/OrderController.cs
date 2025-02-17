using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using KKShop.BusinessLayers;
using KKShop.DomainModels;
using KKShop.Web.AppCodes;
using KKShop.Web.Models;

namespace KKShop.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE}")]
    public class OrderController : Controller
    {
        public const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
        public const int PAGE_SIZE = 20;

        //Số mặt hàng được hiển thị trên một trang tìm kiếm mặt hàng để đưa vào đơn hàng
        private const int PRODUCT_PAGE_SIZE = 5;
        // Tên biến session lưu điều kiện tìm kiếm mặt hàng khi lập  đơn hàng 
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        // Tên biến sesion lưu giỏ hàng
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                // Mặc định thời gian tìm kiếm là khoảng thời gian 7 ngày gần nhất (ví dụ: từ hôm nay trừ đi 7 ngày đến hôm nay)
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(condition);
        }

        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");

            var model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }

        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            //Tìm kiếm và trả kết quả tìm kiếm về cho search product
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize,  condition.SearchValue ?? "");

            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };

            return View(model);
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var orderDetail = OrderDataService.GetOrderDetail(id, productId);
            return View(orderDetail);
        }

        public IActionResult UpdateDetail(OrderDetail data)
        {
            if (data.SalePrice < 0)
                ModelState.AddModelError(nameof(data.SalePrice), "Giá bán không hợp lệ!");
            if (data.Quantity <= 0)
                ModelState.AddModelError(nameof(data.Quantity), "Số lượng không hợp lệ!");
            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, render lại modal cùng lỗi
                return PartialView("EditDetail", data);
            }

            // Nếu thành công, xử lý dữ liệu và trả về JSON redirect URL
            OrderDataService.SaveOrderDetail(data.OrderID, data.ProductID, data.Quantity, data.SalePrice);
            return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = data.OrderID }) });

        }


        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
            {
                return Json("Giá bán và số lượng không hợp lệ");
            }
            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
            {
            // Nếu tìm thấy sản phẩm, xóa khỏi giỏ hàng
                shoppingCart.RemoveAt(index);
            }
            // Nếu không tìm thấy sản phẩm, trả về thông báo lỗi hoặc giá trị khác
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count() == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            }
            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            }
            int employeeID = int.Parse(User.GetUserData().UserId); //TODO: Thay bởi ID của nhân  viên đang login vào hệ thống
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
        public IActionResult Delete(int id)
        {
            OrderDataService.DeleteOrder(id);
            return RedirectToAction("Index");
        }
        public IActionResult DeleteDetail(int id, int productID)
        {
            OrderDataService.DeleteOrderDetail(id, productID);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Accept(int id)
        {
            OrderDataService.AcceptOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Shipping(int id)
        {
            var data = OrderDataService.GetOrder(id);
            return View(data);
        }
        public IActionResult UpdateShipping(Order data)
        {
            if (data.ShipperID == 0)
                ModelState.AddModelError(nameof(data.ShipperID), "Vui lòng chọn người giao hàng!");
            if (!ModelState.IsValid)
                return PartialView("Shipping", data);
            OrderDataService.ShipOrder(data.OrderID, data.ShipperID ?? 0);
            return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = data.OrderID }) });
        }
        public IActionResult Reject(int id)
        {
            OrderDataService.RejectOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Cancel(int id)
        {
            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Finish(int id)
        {
            OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

    }
}