using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibTeam.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class TaiKhoanController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;

        public TaiKhoanController(
            UserManager<AppUserModel> userManager,
            DataContext context,
            SignInManager<AppUserModel> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(tv => tv.UserName.Contains(searchTerm)
                                      || tv.PhoneNumber.Contains(searchTerm)
                                      || tv.Email.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var users = await query.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

            // Chuyển đổi từ AppUserModel sang TaiKhoanModel
            var taiKhoanModels = new List<TaiKhoanModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                taiKhoanModels.Add(new TaiKhoanModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    SDT = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "NhanVien" // Mặc định là NhanVien nếu chưa có role
                });
            }

            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;

            return View(taiKhoanModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaiKhoanModel model)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(model.UserName) ||
                    string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.SDT) ||
                    string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.Role))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
                }

                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng" });
                }

                // Kiểm tra username đã tồn tại chưa
                var existingUserName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserName != null)
                {
                    return Json(new { success = false, message = "Tên đăng nhập đã được sử dụng" });
                }

                // Tạo user mới
                var user = new AppUserModel
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.SDT,
                    EmailConfirmed = true // Để user có thể đăng nhập ngay
                };

                // Tạo user với mật khẩu
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Thêm role cho user
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return Json(new { success = true, message = "Thêm người dùng thành công" });
                }

                // Trả về lỗi nếu có
                var errors = result.Errors.Select(e => e.Description);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi thêm người dùng: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaiKhoanModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy người dùng" });
                    }

                    // Cập nhật thông tin cơ bản
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.SDT;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
                    }

                    // Cập nhật role nếu có thay đổi
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = currentRoles.FirstOrDefault();

                    if (currentRole != model.Role)
                    {
                        if (currentRole != null)
                        {
                            await _userManager.RemoveFromRoleAsync(user, currentRole);
                        }
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    return Json(new { success = true, message = "Cập nhật thành công" });
                }

                return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật thông tin: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa người dùng: " + ex.Message });
            }
        }

        public async Task<IActionResult> Logout(string returnUrl = "/Login")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var model = new TaiKhoanModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    SDT = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "NhanVien"
                };

                return Json(new { success = true, data = model });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lấy thông tin người dùng: " + ex.Message });
            }
        }
    }
}
