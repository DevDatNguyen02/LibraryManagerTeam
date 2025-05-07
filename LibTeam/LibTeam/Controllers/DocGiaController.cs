using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace LibTeamrollers
{
    [Authorize(Roles = "QuanTriVien")]
    public class DocGiaController : Controller
    {
        private readonly DataContext _context;

        public DocGiaController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.DocGias.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(dg => dg.HoTen.Contains(searchTerm)
                                       || dg.DiaChi.Contains(searchTerm)
                                       || dg.SDT.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var docGias = await query.OrderBy(dg => dg.HoTen)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;

            return View(docGias);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DocGia docGia)
        {
            if (string.IsNullOrWhiteSpace(docGia.HoTen) ||
                string.IsNullOrWhiteSpace(docGia.DiaChi) ||
                string.IsNullOrWhiteSpace(docGia.SDT))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin" });
            }

            try
            {
                _context.DocGias.Add(docGia);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm độc giả thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DocGia docGia)
        {
            var existing = await _context.DocGias.FindAsync(docGia.SoTheDG);
            if (existing == null)
                return Json(new { success = false, message = "Không tìm thấy độc giả" });

            existing.HoTen = docGia.HoTen;
            existing.DiaChi = docGia.DiaChi;
            existing.SDT = docGia.SDT;

            try
            {
                _context.DocGias.Update(existing);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var docGia = await _context.DocGias.FindAsync(id);
            if (docGia == null)
                return Json(new { success = false, message = "Không tìm thấy độc giả" });

            try
            {
                _context.DocGias.Remove(docGia);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var docGia = await _context.DocGias.FindAsync(id);
            if (docGia == null)
                return Json(new { success = false, message = "Không tìm thấy độc giả" });

            return Json(new { success = true, data = docGia });
        }
    }
}
