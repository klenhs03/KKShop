using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace KKShop.Shop.AppCodes
{
    /// <summary>
    /// Những thông tin cần thể hiện trong "danh tính" của người dùng
    /// </summary>
    public class WebUserData
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Tạo ra chứng nhận để ghi nhận danh tính của người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal ()
        {
            // Danh sách các Claim chứa các thông tin liên quan đến "danh tinh" người dùng
            List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Photo), Photo)
            };
            if (Roles != null)
                foreach (var role in Roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

            // Tạo Identity dựa trên các thông tin có trong danh sách các Claim
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Tạo Principal (giấy chứng nhận => thẻ sinh viên )
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
