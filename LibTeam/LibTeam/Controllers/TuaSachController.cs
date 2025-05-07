using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibTeam.Controllers
{
    public class TuaSachController : Controller
    {
        private readonly DataContext _context;
        public TuaSachController(DataContext context)
        {
            _context = context;
        }

        // GET: /TuaSach
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.TuaSaches.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(ts => ts.TenTuaSach.Contains(search));

            var totalItems = await query.CountAsync();
            var items = await query
                .OrderBy(ts => ts.TenTuaSach)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        // GET: /TuaSach/Details/5
        [HttpGet]
        public async Task<JsonResult> Details(int id)
        {
            var ts = await _context.TuaSaches.FindAsync(id);
            if (ts == null)
                return Json(new { success = false, message = "Không tìm thấy tựa sách." });

            return Json(new
            {
                success = true,
                tuaSachID = ts.TuaSachID,
                tenTuaSach = ts.TenTuaSach,
                moTa = ts.MoTa
            });
        }

        // POST: /TuaSach/Create
        [HttpPost]
        public async Task<JsonResult> Create([FromForm] TuaSach model)
        {
            // Chỉ bỏ qua navigation property
            ModelState.Remove(nameof(model.CuonSaches));

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .SelectMany(kv => kv.Value.Errors)
                    .Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join("; ", errors) });
            }

            // TuaSachID được DB tự sinh vì Identity
            _context.TuaSaches.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Thêm tựa sách thành công." });
        }

        // POST: /TuaSach/Edit/5
        [HttpPost]
        public async Task<JsonResult> Edit(int id, [FromForm] TuaSach model)
        {
            // Bỏ qua navigation property
            ModelState.Remove(nameof(model.CuonSaches));

            if (id != model.TuaSachID)
                return Json(new { success = false, message = "ID không khớp." });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            try
            {
                _context.TuaSaches.Update(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công." });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.TuaSaches.AnyAsync(e => e.TuaSachID == id))
                    return Json(new { success = false, message = "Tựa sách đã bị xóa trước đó." });
                throw;
            }
        }

        // POST: /TuaSach/DeleteConfirmed/5
        [HttpPost]
        public async Task<JsonResult> DeleteConfirmed(int id)
        {
            var ts = await _context.TuaSaches.FindAsync(id);
            if (ts == null)
                return Json(new { success = false, message = "Không tìm thấy tựa sách." });

            _context.TuaSaches.Remove(ts);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa tựa sách thành công." });
        }
    }
}
