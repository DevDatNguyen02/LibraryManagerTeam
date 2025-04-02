using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class TuaSach
    {
        [Key]
        public int TuaSachID { get; set; }

        [Required, StringLength(150)]
        public string TenTuaSach { get; set; }

        [Required]
        public int NhaXuatBanID { get; set; }  // Khóa ngoại đến NhaXuatBan

        // Quan hệ điều hướng
        public NhaXuatBan NhaXuatBan { get; set; }

        public ICollection<CuonSach> CuonSaches { get; set; } = new HashSet<CuonSach>();
        public ICollection<SachTacGia> SachTacGias { get; set; } = new HashSet<SachTacGia>();
        public ICollection<MuonSach> MuonSaches { get; set; } = new HashSet<MuonSach>();
    }
}
