using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class CuonSach
    {
        [Key]
        public int CuonSachID { get; set; }  // Khóa chính tự tăng

        [Required]
        public int TuaSachID { get; set; }   // Khóa ngoại đến TuaSach

        [Required]
        public int ThuVienID { get; set; }   // Khóa ngoại đến ThuVien

        [Required]
        public int SoLuong { get; set; }

        // Quan hệ điều hướng
        public TuaSach TuaSach { get; set; }
        public ThuVien ThuVien { get; set; }
    }
}
