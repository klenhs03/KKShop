using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KKShop.BusinessLayers;
using KKShop.DomainModels;
using KKShop.Web.AppCodes;
using KKShop.Web.Models;

namespace KKShop.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}, {WebUserRoles.EMPLOYEE}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string SUPPLIER_SEARCH_CONDITION = "SupplierSearchCondition";


        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListOfSuppliers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            SupplierSearchResult model = new SupplierSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_CONDITION, condition);

            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var data = new Supplier()
            {
                SupplierID = 0,
            };
            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var data = CommonDataService.GetSupplier(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Save(Supplier data)
        {
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";

            // Kiểm tra các dữ liệu đầu vào để phát hiện các trường hợp không hợp lệ
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên không được để rỗng");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Provice))
                ModelState.AddModelError(nameof(data.Provice), "Vui lòng chọn tỉnh thành");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập địa chỉ Email");
            //TODO: Kiểm tra dữ liệu đầu vào đúng hay không ? ==> View->Task list
            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            if (data.SupplierID == 0)
            {
                int id = CommonDataService.AddSupplier(data);
                if (id <= 0)
                {
                    ModelState.AddModelError("InvalidEmail", "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateSupplier(data);
                if (!result)
                {
                    ModelState.AddModelError("InvalidEmail", "Email bị trùng");
                    return View("Edit", data);
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            // Xử lí theo phương thức post
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            // Xử lí theo phương thức Get
            var data = CommonDataService.GetSupplier(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
    }
}
