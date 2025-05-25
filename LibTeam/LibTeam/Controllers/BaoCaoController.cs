using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LibTeam.DbContext;

namespace LibTeam.Controllers
{
    [Authorize(Roles = "QuanTriVien")]
    public class BaoCaoController : Controller
    {
        private readonly DataContext _context;

        public BaoCaoController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetThongKeMuonSach(string loaiThongKe)
        {
            var today = DateTime.Today;
            var query = _context.MuonSaches
                .Include(ms => ms.CuonSach)
                .ThenInclude(cs => cs.TuaSach)
                .AsQueryable();

            // Lọc dữ liệu theo loại thống kê
            switch (loaiThongKe.ToLower())
            {
                case "ngay":
                    query = query.Where(ms => ms.NgayMuon.Date == today);
                    break;
                case "tuan":
                    var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                    var endOfWeek = startOfWeek.AddDays(6);
                    query = query.Where(ms => ms.NgayMuon.Date >= startOfWeek && ms.NgayMuon.Date <= endOfWeek);
                    break;
                case "thang":
                    query = query.Where(ms => ms.NgayMuon.Month == today.Month && ms.NgayMuon.Year == today.Year);
                    break;
                case "tuasach":
                    // Không cần lọc theo thời gian cho thống kê tựa sách
                    break;
                default:
                    return Json(new { success = false, message = "Loại thống kê không hợp lệ" });
            }

            if (loaiThongKe.ToLower() == "tuasach")
            {
                // Thống kê theo tựa sách
                var thongKeTuaSach = await query
                    .GroupBy(ms => new {
                        TuaSachId = ms.CuonSach.TuaSach.TuaSachID,
                        TenTuaSach = ms.CuonSach.TuaSach.TenTuaSach
                    })
                    .Select(g => new
                    {
                        TuaSachId = g.Key.TuaSachId,
                        TenTuaSach = g.Key.TenTuaSach,
                        SoLuotMuon = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuotMuon)
                    .ToListAsync();

                return Json(new { success = true, data = thongKeTuaSach });
            }
            else
            {
                // Thống kê theo cuốn sách
                var thongKe = await query
                    .GroupBy(ms => new { ms.CuonSach.CuonSachID, ms.CuonSach.TenSach })
                    .Select(g => new
                    {
                        TenSach = g.Key.TenSach,
                        SoLuotMuon = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuotMuon)
                    .Take(10)
                    .ToListAsync();

                return Json(new { success = true, data = thongKe });
            }
        }
    }
}