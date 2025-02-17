using Dapper;
using KKShop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.DataLayers.SQLServer
{
    public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Employees where Email = @Email)
                                 select 1
                            else
                                 begin

                            insert into Employees (FullName, BirthDate, Address, Phone, Photo, Email, IsWorking)
                            values  (@FullName, @BirthDate, @Address, @Photo, @Phone, @Email, @IsWorking);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                            end";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    Address = data.Address ?? "",
                    Photo = data.Photo ?? "",
                    Email = data.Email ?? "",
                    Phone = data.Phone ?? "",
                    BirthDate = data.BirthDate,
                    IsWorking = data.IsWorking
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
	                        from  Employees
	                        where (FullName like @searchValue) 
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
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
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
                var sql = @"if exists(select * from Orders where EmployeeID = @EmployeeID)
                                      select 1 
                            else
                                      select 0";
                var parameters = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            searchValue = $"%{searchValue}%"; // Tìm kiếm tương đối với LIKE
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
                                  select *,
	                                      row_number() over(order by FullName) as RowNumber
	                              from  Employees
	                              where (FullName like @searchValue)
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
                data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF NOT EXISTS (SELECT * FROM Employees WHERE EmployeeID <> @EmployeeID AND Email = @Email)
                            BEGIN
                                UPDATE Employees
                                SET FullName = @FullName,
                                    BirthDate = @BirthDate,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email, 
                                    Photo = @Photo,
                                    IsWorking = @IsWorking 
                                WHERE EmployeeID = @EmployeeID;
                            END";
                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    BirthDate = data.BirthDate,
                    IsWorking = data.IsWorking,
                    EmployeeID = data.EmployeeID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        Employee? ICommonDAL<Employee>.Get(int id)
        {
            Employee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id
                };
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }
    }

}
