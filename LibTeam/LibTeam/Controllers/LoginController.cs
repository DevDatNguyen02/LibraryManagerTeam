using LibTeam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibTeam.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUserModel> _signInManager;

        public LoginController(SignInManager<AppUserModel> signInManager)
        {
            _signInManager = signInManager;
        }

        // GET: Login
        public IActionResult Index()
        {
            return View(new TaiKhoanModel());
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(TaiKhoanModel loginInfo)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", loginInfo); 
            }

            var result = await _signInManager.PasswordSignInAsync(loginInfo.UserName, loginInfo.Password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "TuaSach");  
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View("Index", loginInfo);
            }
        }
    }
}
