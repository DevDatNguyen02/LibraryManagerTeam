using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibTeam.Models
{
    public class CuonSach
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CuonSachID { get; set; }

        [Required]
        public string TenSach { get; set; }

        // FK đến TuaSach
        [ForeignKey("TuaSach")]
        public int? TuaSachID { get; set; }
        public TuaSach TuaSach { get; set; }

        // Thông tin tác giả
        [Required]
        public string TenTacGia { get; set; }

        // Thông tin Nhà xuất bản
        [Required]
        public string TenNXB { get; set; }

        [Required]
        public int SoLuong { get; set; }

        [Required]
        public string TrangThai { get; set; } = "SanSang";

        // Quan hệ mượn/trả
        public ICollection<MuonSach> MuonSaches { get; set; }
    }
}
