using System.ComponentModel.DataAnnotations;

namespace LibTeam.Models
{
    public class MuonSach
    {
        [Key]
        public int MuonSachID { get; set; }  // Khóa chính tự tăng

        [Required]
        public int TuaSachID { get; set; }   // Khóa ngoại đến TuaSach

        [Required]
        public int ThuVienID { get; set; }   // Khóa ngoại đến ThuVien

        [Required]
        public int SoTheDG { get; set; }     // Khóa ngoại đến DocGia

        [Required]
        public DateTime NgayMuon { get; set; }

        public DateTime? NgayTra { get; set; }

        // Quan hệ điều hướng
        public TuaSach TuaSach { get; set; }
        public ThuVien ThuVien { get; set; }
        public DocGia DocGia { get; set; }
    }
}
