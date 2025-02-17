using Azure;
using Microsoft.Identity.Client;
using KKShop.DataLayers;
using KKShop.DataLayers.SQLServer;
using KKShop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ISimpleSelectDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Shipper>  shipperDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            ///string connectionString = "server=.\\SQLEXPRESS01;user id=sa;password=123;database=LiteCommerceDB;TrustServerCertificate=true";

            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);

            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);

        }

        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của một khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer (int id)
        {
            return customerDB.Get(id);
        }

        /// <summary>
        /// Bổ sung một khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer (Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Cập nhật khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer (Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// Xóa khách hàng có mã là id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool DeleteCustomer (int id)
        {
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem một khách hàng liệu có các dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer (int id)
        {
            return customerDB.InUsed(id);
        }




        /////////////// CATEGORY ///////////
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy thông tin của một khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bổ sung một Category
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Cập nhật Category
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa Category có mã là id
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem một category liệu có các dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }




        //////////// SUPPERLIER /////////////////
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }




        /////////////// SHIPPER /////////////////

        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
       
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }



        ////////////// EMPLOYEEE ////////////////
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }

        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }

        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }




    }
}
