using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace LibTeamrollers
{
    [Authorize]
    public class DocGiaController : Controller
    {
        private readonly DataContext _context;

        public DocGiaController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm, int page = 1, int pageSize = 5, int? ST = null)
        {
            // Lấy danh sách thư viện từ cơ sở dữ liệu
            var query = _context.DocGias.AsQueryable();

            // Lọc theo tên thư viện hoặc địa chỉ nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(dg => dg.TenDG.Contains(searchTerm) || dg.DiaChiDG.Contains(searchTerm) || dg.SDTDG.Contains(searchTerm));
            }
            // Tìm kiếm theo số thẻ độc giả
            if (ST.HasValue)
            {
                query = query.Where(dg => dg.SoTheDG == ST.Value);
            }

            // Tổng số bản ghi (để hỗ trợ phân trang)
            var totalItems = query.Count();

            // Phân trang
            var docGiaList = query
                .OrderBy(dg => dg.TenDG)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gửi thông tin phân trang đến View
            ViewBag.TotalItems = totalItems;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.ST = ST;

            return View(docGiaList);
        }
        [HttpPost]
        public async Task<IActionResult> Create(DocGia docGia)
        {
            try
            {
                _context.Add(docGia);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "lỗi: " + ex.Message);
            }

            return View(docGia);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int SoTheDG, string TenDG, string DiaChiDG, string SDTDG)
        {
            if (ModelState.IsValid)
            {
                var docGias = await _context.DocGias.FindAsync(SoTheDG);
                if (docGias == null)
                {
                    return NotFound();
                }
                docGias.TenDG = TenDG;
                docGias.DiaChiDG = DiaChiDG;
                docGias.SDTDG = SDTDG;

                _context.Update(docGias);
                await _context.SaveChangesAsync();

                // Chuyển hướng sau khi lưu thành công
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var docGias = await _context.DocGias.FindAsync(id);
            if (docGias == null)
            {
                return NotFound();
            }

            _context.DocGias.Remove(docGias);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
