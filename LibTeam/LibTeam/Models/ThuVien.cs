using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class ThuVien
    {
        [Key]
        public int MaThuVien { get; set; }

        [Required, StringLength(150)]
        public string TenThuVien { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        // Quan hệ 1-n: Một thư viện có nhiều cuốn sách và lượt mượn
        public ICollection<CuonSach> CuonSaches { get; set; } = new HashSet<CuonSach>();
        public ICollection<MuonSach> MuonSaches { get; set; } = new HashSet<MuonSach>();
    }
}
