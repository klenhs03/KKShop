using Dapper;
using KKShop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @" if exists (SELECT * FROM Suppliers WHERE Email = @Email)
                                select -1
                             else
                                begin
                                    insert into Suppliers (SupplierName, ContactName, Provice, Address, Phone, Email)
                                    values (@SupplierName, @ContactName, @Provice, @Address, @Phone, @Email);
                                    select cast (SCOPE_IDENTITY() as int);
                                end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%"; // "%" + searchValue + "%"
            using (var connection = OpenConnection())
            {
                var sql = @"select Count(*)
	                        from  Suppliers
	                        where (SupplierName like @searchValue) 
                           ";
                var parameters = new
                {
                    searchValue
                };
                count = connection.ExecuteScalar<int>(sql: sql, parameters, commandType: System.Data.CommandType.Text);
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierID = @SupplierID)
                                      select 1 
                            else
                                      select 0";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            searchValue = $"%{searchValue}%"; // Tìm kiếm tương đối với LIKE
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
                                  select *,
	                                      row_number() over(order by SupplierName) as RowNumber
	                              from  Suppliers
	                              where (SupplierName like @searchValue) or (ContactName like @searchValue)
	                                     ) as t
                            where (@pageSize = 0)
                                  or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page, // bên trái: tên tham số trong câu lệnh sql, bên phải: giá trị truyền vào cho tham số
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Supplier data)
         {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers WHERE SupplierID<> @SupplierID AND Email = @Email)
                            begin
                            update Suppliers
                            set SupplierName = @SupplierName,
                                ContactName = @ContactName,
	                            Provice = @Provice,
	                            Address = @Address,
	                            Phone = @Phone,
	                            Email = @Email
                            where SupplierID = @SupplierID;
                            end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    SupplierID = data.SupplierID

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        Supplier? ICommonDAL<Supplier>.Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }
    }
}


    
