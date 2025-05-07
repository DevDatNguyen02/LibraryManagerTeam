using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibTeam.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class CuonSachController : Controller
    {
        private readonly DataContext _context;
        public CuonSachController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.CuonSaches
                                .Include(cs => cs.TuaSach)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(cs =>
                    cs.TenSach.Contains(search) ||
                    cs.TenTacGia.Contains(search) ||
                    cs.TenNXB.Contains(search));
                ViewBag.Search = search;
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(cs => cs.TenSach)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TuaSachList = await _context.TuaSaches.ToListAsync();

            return View(items);
        }

        [HttpGet]
        public async Task<JsonResult> Details(int id)
        {
            var cs = await _context.CuonSaches.FindAsync(id);
            if (cs == null)
                return Json(new { success = false, message = "Không tìm thấy cuốn sách." });

            return Json(new
            {
                success = true,
                cuonSachID = cs.CuonSachID,
                tenSach = cs.TenSach,
                tuaSachID = cs.TuaSachID,
                tenTacGia = cs.TenTacGia,
                tenNXB = cs.TenNXB,
                soLuong = cs.SoLuong,
                trangThai = cs.TrangThai
            });
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromForm] CuonSach model)
        {
            // Bỏ qua CuonSachID vì là bản ghi mới, EF sẽ tự động tạo
            ModelState.Remove(nameof(model.CuonSachID));
            ModelState.Remove(nameof(model.MuonSaches));
            ModelState.Remove(nameof(model.TuaSach));

            // Xử lý TuaSachID: Nếu giá trị từ form là "", gán thành null
            var tuaSachIdString = HttpContext.Request.Form["TuaSachID"].ToString();
            if (string.IsNullOrWhiteSpace(tuaSachIdString))
            {
                model.TuaSachID = null;
            }

            // Kiểm tra TuaSachID bắt buộc
            if (model.TuaSachID == null)
            {
                return Json(new { success = false, message = "Vui lòng chọn tựa sách." });
            }

            // Kiểm tra TrangThai bắt buộc
            if (string.IsNullOrWhiteSpace(model.TrangThai))
            {
                return Json(new { success = false, message = "Vui lòng chọn trạng thái." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors) });
            }

            _context.CuonSaches.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Thêm cuốn sách thành công." });
        }

        [HttpPost]
        public async Task<JsonResult> Edit(int id, [FromForm] CuonSach model)
        {
            ModelState.Remove(nameof(model.MuonSaches));
            ModelState.Remove(nameof(model.TuaSach));

            if (id != model.CuonSachID)
                return Json(new { success = false, message = "ID không khớp." });

            // Xử lý TuaSachID: Nếu giá trị từ form là "", gán thành null
            var tuaSachIdString = HttpContext.Request.Form["TuaSachID"].ToString();
            if (string.IsNullOrWhiteSpace(tuaSachIdString))
            {
                model.TuaSachID = null;
            }

            // Kiểm tra TuaSachID bắt buộc
            if (model.TuaSachID == null)
            {
                return Json(new { success = false, message = "Vui lòng chọn tựa sách." });
            }

            // Kiểm tra TrangThai bắt buộc
            if (string.IsNullOrWhiteSpace(model.TrangThai))
            {
                return Json(new { success = false, message = "Vui lòng chọn trạng thái." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors) });
            }

            try
            {
                _context.CuonSaches.Update(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công." });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.CuonSaches.AnyAsync(e => e.CuonSachID == id))
                    return Json(new { success = false, message = "Cuốn sách đã bị xóa trước đó." });
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            var cs = await _context.CuonSaches.FindAsync(id);
            if (cs == null)
                return Json(new { success = false, message = "Không tìm thấy cuốn sách." });

            _context.CuonSaches.Remove(cs);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa cuốn sách thành công." });
        }
    }
}
