using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KKShop.Shop.Controllers
{
    
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
