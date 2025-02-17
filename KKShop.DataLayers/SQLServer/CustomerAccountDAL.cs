using Dapper;
using KKShop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserId,
                                   Email as UserName,
                                   CustomerName as DisplayName,
                                   N'' as Photo,
                                   N'' as RoleNames
                            from   Customers
                            where Email = @Email and Password = @Password ";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        public int CheckOldePassword(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
