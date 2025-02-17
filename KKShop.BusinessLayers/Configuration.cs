using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.BusinessLayers
{
    public static class Configuration
    {
        private static string connectionString = "";

        /// <summary>
        /// Khởi tạo cấu hình cho BusinessLayer
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }

        /// <summary>
        /// Chuỗi tham số kết nối CSDL
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }    
        }

    }
}
