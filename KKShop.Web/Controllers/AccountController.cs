﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using KKShop.BusinessLayers;
using KKShop.Web.AppCodes;
using KKShop.DomainModels;
using KKShop.DataLayers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KKShop.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }
            //TODO: Kiểm tra xe username và password (của Employee) có đúng không?
            var userAccount = UserAccountService.Authorise(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            // Đăng nhập thành công
            //1. Tọa thông tin người dung
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList() //Split phân tách một chuỗi thành một mảng các chuỗi con dựa trên ký tự phân tách được chỉ định
            };
            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword()
        {

            var data = new ChangePassword();
            return View(data);

        }
        public IActionResult HandleChangePassword(ChangePassword data)
        {
            // Kiểm tra mật khẩu cũ đúng hay không ?
            if (UserAccountService.CheckOldPassword(User.GetUserData().UserName, data.OldPassword) == 0) //Kiểm tra mật khẩu cũ
                ModelState.AddModelError(nameof(data.OldPassword), "Mật khẩu cũ không đúng");
            // Ktra mật khẩu mới
            if (string.IsNullOrEmpty(data.NewPassword) || data.NewPassword.Length < 6)
                ModelState.AddModelError(nameof(data.NewPassword), "Mật khẩu quá ngắn (tối thiểu phải có 6 kí tự!)");
            // Ktra mkxn
            else if (!data.NewPassword.Equals(data.ConfirmPassword))
                ModelState.AddModelError(nameof(data.ConfirmPassword), "Xác nhận mật khẩu không đúng");

            // Nếu đúng hết
            if (ModelState.IsValid)
            {
                if (UserAccountService.ChangePassword(User.GetUserData().UserName, data.NewPassword))
                    data.SuccessMessage = "Bạn đã thay đổi mật khẩu thành công";
            }
            return View("ChangePassword", data);
        }
        public IActionResult AccessDenined()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


    }
}
