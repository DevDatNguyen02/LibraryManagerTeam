using LibTeam.DbContext;
using LibTeam.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibTeam.Controllers
{
    public class MuonSachController : Controller
    {
        private readonly DataContext _context;

        public MuonSachController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, DateTime? dateMuon, DateTime? dateTra, int page = 1, int pageSize = 5)
        {
            var query = _context.MuonSaches
                               .Include(ms => ms.CuonSach)
                               .ThenInclude(cs => cs.TuaSach)
                               .Include(ms => ms.DocGia)
                               .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(ms =>
                    ms.CuonSach.TenSach.Contains(search) ||
                    ms.CuonSach.TuaSach.TenTuaSach.Contains(search) ||
                    ms.DocGia.HoTen.Contains(search));
            }

            if (dateMuon.HasValue)
            {
                query = query.Where(ms => ms.NgayMuon.Date == dateMuon.Value.Date);
            }

            if (dateTra.HasValue)
            {
                query = query.Where(ms => ms.NgayTra.HasValue && ms.NgayTra.Value.Date == dateTra.Value.Date);
            }

            int totalItems = query.Count();

            var muonSachs = query
                .OrderBy(ms => ms.NgayMuon)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.DateMuon = dateMuon?.ToString("yyyy-MM-dd");
            ViewBag.DateTra = dateTra?.ToString("yyyy-MM-dd");
            ViewBag.ListCuonSach = _context.CuonSaches.Include(cs => cs.TuaSach).ToList();
            ViewBag.ListDocGia = _context.DocGias.ToList();

            return View(muonSachs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MuonSach muonSach)
        {
            ModelState.Remove(nameof(muonSach.CuonSach));
            ModelState.Remove(nameof(muonSach.DocGia));

            var cuonSach = _context.CuonSaches.FirstOrDefault(cs => cs.CuonSachID == muonSach.CuonSachID);
            if (cuonSach == null || cuonSach.SoLuong <= 0 || cuonSach.TrangThai != "SanSang")
            {
                TempData["Error"] = "Cuốn sách không thể mượn được.";
                return RedirectToAction(nameof(Index));
            }

            if (!_context.DocGias.Any(dg => dg.SoTheDG == muonSach.SoTheDG))
            {
                TempData["Error"] = "Độc giả không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            cuonSach.SoLuong--;
            _context.MuonSaches.Add(muonSach);
            _context.SaveChanges();

            TempData["Success"] = "Mượn sách thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateNgayTra(int cuonSachID, int soTheDG, DateTime ngayTra)
        {
            var muonSach = _context.MuonSaches
                .Include(ms => ms.CuonSach)
                .FirstOrDefault(ms => ms.CuonSachID == cuonSachID && ms.SoTheDG == soTheDG);

            if (muonSach == null)
            {
                TempData["Error"] = "Không tìm thấy phiếu mượn.";
                return RedirectToAction(nameof(Index));
            }

            if (ngayTra.Date < muonSach.NgayMuon.Date)
            {
                TempData["Error"] = "Ngày trả không được trước ngày mượn.";
                return RedirectToAction(nameof(Index));
            }

            if (muonSach.NgayTra == null)
            {
                muonSach.CuonSach.SoLuong++;
            }

            muonSach.NgayTra = ngayTra;
            _context.SaveChanges();

            TempData["Success"] = "Trả sách thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int cuonSachID, int soTheDG)
        {
            var muonSach = _context.MuonSaches
                .FirstOrDefault(ms => ms.CuonSachID == cuonSachID && ms.SoTheDG == soTheDG);

            if (muonSach == null)
            {
                TempData["Error"] = "Không tìm thấy phiếu mượn.";
                return RedirectToAction(nameof(Index));
            }

            if (muonSach.NgayTra == null)
            {
                TempData["Error"] = "Chưa trả sách, không thể xóa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.MuonSaches.Remove(muonSach);
                _context.SaveChanges();
                TempData["Success"] = "Xóa phiếu mượn thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa phiếu mượn: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
