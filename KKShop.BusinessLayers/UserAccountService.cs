using Azure.Identity;
using KKShop.BusinessLayers;
using KKShop.DataLayers;
using KKShop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(Configuration.ConnectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorise(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
            {
                return employeeAccountDB.Authorize(username, password);
            }
            else
            {
                return customerAccountDB.Authorize(username, password);
            }
        }
        public static int CheckOldPassword(string username, string password)
        {
            return employeeAccountDB.CheckOldePassword(username, password);
        }
        public static bool ChangePassword(string username, string password)
        {
            return employeeAccountDB.ChangePassword(username, password);
        }
    }
    public enum UserTypes
    {
        Employee,
        Customer
    }
}
